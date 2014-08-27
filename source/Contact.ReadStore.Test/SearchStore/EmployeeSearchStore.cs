using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using AutoMapper;
using Contact.Domain.Events.Employee;
using Contact.Infrastructure;
using Raven.Client;

namespace Contact.ReadStore.Test.SearchStore
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


            handler.RegisterHandler<EmployeeCreated>(FillRaven);
            
           
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

        private void FillRaven(EmployeeCreated obj)
        {

            var searchModel = Mapper.Map<EmployeeCreated, EmployeeSearchModel>(obj);
            using (var session = documentStore.OpenSession())
            {
                session.Store(searchModel);
                session.SaveChanges();
            }
        }
    }
}