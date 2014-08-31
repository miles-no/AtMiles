using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Contact.Domain;
using Contact.Domain.Events.Employee;
using Contact.Domain.Events.Import;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
using Raven.Client;

namespace Contact.ReadStore.SearchStore
{
    public class EmployeeSearchStore
    {
        private readonly IDocumentStore _documentStore;

        public EmployeeSearchStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<EmployeeCreated>(HandleEmployeeCreated); 
            handler.RegisterHandler<ImportedFromCvPartner>(HandleImportCvPartner);
           
            handler.RegisterHandler<BusyTimeAdded>(HandleBusyTimeAdded);
            handler.RegisterHandler<BusyTimeConfirmed>(HandleBusyTimeConfirmed);
            handler.RegisterHandler<BusyTimeRemoved>(HandleBusyTimeRemoved);
            handler.RegisterHandler<Domain.Events.Company.EmployeeMovedToNewOffice>(HandleEmployeeMovedToNewOffice);
        }

        private static List<string> CreateKeyQalifications(IEnumerable<CvPartnerKeyQualification> keyQualifications)
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

        private static Tag[] CreateCompetency(IEnumerable<CvPartnerTechnology> technologies)
        {
            var res = new List<Tag>();
            if (technologies == null) return res.ToArray();


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
            return res.ToArray();
        }


        private void HandleImportCvPartner(ImportedFromCvPartner ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var existing = session.Load<EmployeeSearchModel>(ev.EmployeeId);
                if (existing != null)
                {
                    existing = Patch(existing, ev);
                }
                else
                {
                    existing = ConvertTo(ev);
                }

                session.Store(existing);
                session.SaveChanges();
            }
        }
        
        private void HandleEmployeeCreated(EmployeeCreated ev)
        {
            if (ev.EmployeeId == Constants.SystemUserId) return; //Do not show SYSTEM user in search

            var searchModel = ConvertTo(ev);
            using (var session = _documentStore.OpenSession())
            {
                session.Store(searchModel);
                session.SaveChanges();
            }
        }

        private void HandleBusyTimeAdded(BusyTimeAdded ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var employee = session.Load<EmployeeSearchModel>(ev.EmployeeId);
                employee = Patch(employee, ev);
                session.Store(employee);
                session.SaveChanges();
            }
        }

        private void HandleBusyTimeRemoved(BusyTimeRemoved ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var employee = session.Load<EmployeeSearchModel>(ev.EmployeeId);
                employee = Patch(employee, ev);
                session.Store(employee);
                session.SaveChanges();
            }
        }

        private void HandleBusyTimeConfirmed(BusyTimeConfirmed ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var employee = session.Load<EmployeeSearchModel>(ev.EmployeeId);
                employee = Patch(employee, ev);
                session.Store(employee);
                session.SaveChanges();
            }
        }

        private void HandleEmployeeMovedToNewOffice(Domain.Events.Company.EmployeeMovedToNewOffice ev)
        {
            using (var session = _documentStore.OpenSession())
            {
                var employee = session.Load<EmployeeSearchModel>(ev.EmployeeId);
                employee = Patch(employee, ev);
                session.Store(employee);
                session.SaveChanges();
            }
        }

        private static EmployeeSearchModel ConvertTo(EmployeeCreated ev)
        {
            var model = new EmployeeSearchModel
            {
                Id = ev.EmployeeId,
                CompanyId = ev.CompanyId,
                OfficeId = ev.OfficeId,
                OfficeName = ev.OfficeName,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                DateOfBirth = ev.DateOfBirth.HasValue ? ev.DateOfBirth.Value : DateTime.MinValue,
                JobTitle = ev.JobTitle,
                PhoneNumber = ev.PhoneNumber,
                Email = ev.Email,
                Thumb = CreateThumb(ev.Photo, 80, 0),
                BusyTimeEntriesConfirmed = DateTime.MinValue,
                Competency = null,
                KeyQualifications = new List<string>(),
                BusyTimeEntries = new List<EmployeeSearchModel.BusyTime>()
            };

            return model;
        }

        private static EmployeeSearchModel ConvertTo(ImportedFromCvPartner ev)
        {
            var model = new EmployeeSearchModel
            {
                Id = ev.EmployeeId,
                CompanyId = ev.CompanyId,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                DateOfBirth = ev.DateOfBirth.HasValue ? ev.DateOfBirth.Value : DateTime.MinValue,
                JobTitle = ev.Title,
                PhoneNumber = ev.Phone,
                Email = ev.Email,
                Thumb = CreateThumb(ev.Photo, 80, 0),
                BusyTimeEntriesConfirmed = DateTime.MinValue,
                Competency = CreateCompetency(ev.Technologies),
                KeyQualifications = CreateKeyQalifications(ev.KeyQualifications),
                BusyTimeEntries = new List<EmployeeSearchModel.BusyTime>()
            };
            return model;
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, ImportedFromCvPartner ev)
        {
            model.Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName);
            model.DateOfBirth = ev.DateOfBirth.HasValue ? ev.DateOfBirth.Value : DateTime.MinValue;
            model.JobTitle = ev.Title;
            model.PhoneNumber = ev.Phone;
            model.Email = ev.Email;
            model.Thumb = CreateThumb(ev.Photo, 80, 0);
            model.Competency = CreateCompetency(ev.Technologies);
            model.KeyQualifications = CreateKeyQalifications(ev.KeyQualifications);
            return model;
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, BusyTimeConfirmed ev)
        {
            model.BusyTimeEntriesConfirmed = ev.Created;
            return model;
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, BusyTimeAdded ev)
        {
            if(model.BusyTimeEntries == null) model.BusyTimeEntries = new List<EmployeeSearchModel.BusyTime>();
            model.BusyTimeEntries.Add(new EmployeeSearchModel.BusyTime{Id = ev.BusyTimeId, Start = ev.Start, End = ev.End, PercentageOccupied = ev.PercentageOccpied, Comment = ev.Comment});
            return model;
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, BusyTimeRemoved ev)
        {
            if (model.BusyTimeEntries != null)
            {
                model.BusyTimeEntries.RemoveAll(b => b.Id == ev.BusyTimeId);
            }
            return model;
        }
        private static EmployeeSearchModel Patch(EmployeeSearchModel model, Domain.Events.Company.EmployeeMovedToNewOffice ev)
        {
            model.OfficeId = ev.NewOfficeId;
            model.OfficeName = ev.NewOfficeName;
            return model;
        }

        private static string CreateThumb(Picture photo, int width, int height = 0)
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