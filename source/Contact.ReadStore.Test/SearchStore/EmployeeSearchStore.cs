using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using AutoMapper;
using Contact.Domain.Events.Employee;
using Contact.Domain.Events.Import;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
using Raven.Abstractions.Data;
using Raven.Client;

namespace Contact.ReadStore.SearchStore
{
    public class EmployeeSearchStore
    {
        private readonly IDocumentStore documentStore;

        public EmployeeSearchStore(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            Mapper.CreateMap<EmployeeCreated, EmployeeSearchModel>()
                //.ForMember(dest => dest.Competency, source => 
                //    source.MapFrom(s => s.Competence != null ? s.Competence.Select(competenceTag=>new Tag{Category = competenceTag.LocalCategory, Competency = competenceTag.LocalSubject, InternationalCategory = competenceTag.InternationalCategory, InternationalCompentency = competenceTag.InternationalSubject}) : null))
                .ForMember(dest => dest.Id, source => source.MapFrom(s=>s.GlobalId))
                .ForMember(dest => dest.Name, source => source.MapFrom(
                    m => m.FirstName + " " +
                         (string.IsNullOrEmpty(m.MiddleName) ? string.Empty : (m.MiddleName + " ")) + m.LastName))
                .ForMember(dest=>dest.Thumb, source => source.MapFrom(s => CreateThumb(s.Photo, 80, 0)));

            Mapper.CreateMap<ImportedFromCvPartner, EmployeeSearchModel>()
             .ForMember(dest => dest.Name, source => source.MapFrom(e => NameService.GetName(e.FirstName, e.MiddleName, e.LastName)))
             .ForMember(dest => dest.Id, source => source.MapFrom(s => s.EmployeeId))
             .ForMember(dest => dest.Competency, source => source.MapFrom(s => CreateCompetency(s.Technologies)))
             .ForMember(dest => dest.KeyQualifications, source => source.MapFrom(s => CreateKeyQalifications(s.KeyQualifications)))
             .ForMember(dest=>dest.Thumb, source => source.MapFrom(s => CreateThumb(s.Photo, 80, 0)))
             .ForMember(dest=>dest.JobTitle, source => source.MapFrom(s => s.Title));

            handler.RegisterHandler<EmployeeCreated>(HandleImportEmployeeCreated); 
            handler.RegisterHandler<ImportedFromCvPartner>(HandleImportCvPartner);
           
            
           
        }

        private List<string> CreateKeyQalifications(IEnumerable<CvPartnerKeyQualification> keyQualifications)
        {
            var res = new List<string>();
            if (keyQualifications != null)
            {
                foreach (var cvPartnerKeyQualification in keyQualifications)
                {
                    if (cvPartnerKeyQualification.Keypoints != null)
                    {
                        foreach (var cvPartnerKeyPoint in cvPartnerKeyQualification.Keypoints)
                        {
                            res.Add(cvPartnerKeyPoint.InternationalDescription);
                            res.Add(cvPartnerKeyPoint.InternationalName);
                            res.Add(cvPartnerKeyPoint.LocalDescription);
                            res.Add(cvPartnerKeyPoint.LocalName);
                        }
                    }
                }
            }
           
            return res.Where(w => string.IsNullOrEmpty(w) == false).Distinct().ToList();
        }

        private Tag[] CreateCompetency(CvPartnerTechnology[] technologies)
        {
            var res = new List<Tag>();
            if (technologies != null)
            {
                var categories = technologies.GroupBy(g =>new {g.LocalCategory,g.InternationalCategory});
                foreach (var category in categories)
                {
                    foreach (var skill in category.ToList().SelectMany(s=>s.Skills))
                    {
                        res.Add(new Tag
                        {
                            Category = category.Key.LocalCategory, 
                            InternationalCategory = category.Key.InternationalCategory, 
                            Competency = skill.LocalName, 
                            InternationalCompentency = skill.InternationalName
                        });
                    }
                }

            }
            return res.ToArray();
        }


        private void HandleImportCvPartner(ImportedFromCvPartner person)
        {
            using (var session = documentStore.OpenSession())
            {

                //TODO: ImportedFromCvPartner should contain company id as well. As long as there are just one company in the system, this is fine
                var existing = session.Query<EmployeeSearchModel, EmployeeSearchModelLookupIndex>().FirstOrDefault(w => w.Email == person.Email);
                if (existing != null)
                {
                    Mapper.Map(person, existing);
                }
                else
                {
                    existing = Mapper.Map<ImportedFromCvPartner, EmployeeSearchModel>(person);
                }

                session.Store(existing);
                session.SaveChanges();
            }
        }
        private void HandleImportEmployeeCreated(EmployeeCreated obj)
        {

            var searchModel = Mapper.Map<EmployeeCreated, EmployeeSearchModel>(obj);
            using (var session = documentStore.OpenSession())
            {
                session.Store(searchModel);
                session.SaveChanges();
            }
        }

        private static string CreateThumb(Domain.ValueTypes.Picture photo, int width, int height = 0)
        {
            if (photo == null)
            {
                return null;
            }
            Stream stream = new MemoryStream(photo.Content);
            using (var srcImage = Image.FromStream(stream))
            {

                // If height is not specified, calculate from width
                if (height == 0)
                {
                    var newheight = (width / (double)srcImage.Width) * srcImage.Height;
                    height = Convert.ToInt32(newheight);
                }

                using (var newImage = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, width, height));
                    byte[] imageBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Convert Image to byte[]
                        if (photo.Extension == "png")
                        {
                            newImage.Save(ms, ImageFormat.Png);
                        }
                        else if (photo.Extension == "jpg" || photo.Extension == "jpeg")
                        {
                            newImage.Save(ms, ImageFormat.Jpeg);
                        }

                        imageBytes = ms.ToArray();
                        var res = "data:image/" + photo.Extension + ";base64," + Convert.ToBase64String(imageBytes);
                        Debug.WriteLine(res);
                        return res;
                    }

                }
            }
        }

    }
}