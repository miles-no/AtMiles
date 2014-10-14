using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Events.Import;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Infrastructure;
using Raven.Client;

namespace no.miles.at.Backend.ReadStore.SearchStore
{
    public class EmployeeSearchStore
    {
        private readonly IDocumentStore _documentStore;

        public EmployeeSearchStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public static string GetRavenId(string id)
        {
            return "employees/" + id;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<EmployeeCreated>(HandleEmployeeCreated); 
            handler.RegisterHandler<ImportedFromCvPartner>(HandleImportCvPartner);
           
            handler.RegisterHandler<BusyTimeAdded>(HandleBusyTimeAdded);
            handler.RegisterHandler<BusyTimeConfirmed>(HandleBusyTimeConfirmed);
            handler.RegisterHandler<BusyTimeRemoved>(HandleBusyTimeRemoved);
            handler.RegisterHandler<BusyTimeUpdated>(HandleBusyTimeUpdated);
            handler.RegisterHandler<DateOfBirthSet>(HandleBirthDateSet);
            handler.RegisterHandler<PrivateAddressSet>(HandlePrivateAddressSet);
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

        private static List<EmployeeSearchModel.Description> CreateDescritpions(IEnumerable<CvPartnerKeyQualification> keyQualifications)
        {
            var res = new List<EmployeeSearchModel.Description>();
            if (keyQualifications != null)
            {
                res.AddRange(keyQualifications.Select(cvPartnerKeyQualification => new EmployeeSearchModel.Description
                {
                    InternationalDescription = cvPartnerKeyQualification.InternationalDescription, LocalDescription = cvPartnerKeyQualification.LocalDescription
                }));
            }
            return res;
        }

        private static Tag[] CreateCompetency(IEnumerable<CvPartnerTechnology> technologies)
        {
            var res = new List<Tag>();
            if (technologies == null) return res.ToArray();


            var categories = technologies.GroupBy(g =>new {g.LocalCategory,g.InternationalCategory});
            foreach (var category in categories)
            {
                res.AddRange(category.ToList().SelectMany(s => s.Skills).Select(skill => new Tag
                {
                    Category = category.Key.LocalCategory, InternationalCategory = category.Key.InternationalCategory, Competency = skill.LocalName, InternationalCompentency = skill.InternationalName
                }));
            }
            return res.ToArray();
        }


        private async Task HandleImportCvPartner(ImportedFromCvPartner ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var existing = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                existing = existing != null ? Patch(existing, ev) : ConvertTo(ev);

                await session.StoreAsync(existing);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleEmployeeCreated(EmployeeCreated ev)
        {
            if (ev.EmployeeId == Constants.SystemUserId) return; //Do not show SYSTEM user in search

            var searchModel = ConvertTo(ev);
            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(searchModel);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeAdded(BusyTimeAdded ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var employee = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                employee = Patch(employee, ev);
                await session.StoreAsync(employee);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeRemoved(BusyTimeRemoved ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var employee = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                employee = Patch(employee, ev);
                await session.StoreAsync(employee);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeConfirmed(BusyTimeConfirmed ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var employee = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                employee = Patch(employee, ev);
                await session.StoreAsync(employee);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeUpdated(BusyTimeUpdated ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var employee = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                employee = Patch(employee, ev);
                await session.StoreAsync(employee);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandlePrivateAddressSet(PrivateAddressSet ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var employee = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                employee = Patch(employee, ev);
                await session.StoreAsync(employee);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBirthDateSet(DateOfBirthSet ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var employee = await session.LoadAsync<EmployeeSearchModel>(GetRavenId(ev.EmployeeId));
                employee = Patch(employee, ev);
                await session.StoreAsync(employee);
                await session.SaveChangesAsync();
            }
        }

        private static EmployeeSearchModel ConvertTo(EmployeeCreated ev)
        {
            var model = new EmployeeSearchModel
            {
                Id = GetRavenId(ev.EmployeeId),
                GlobalId = ev.EmployeeId,
                CompanyId = ev.CompanyId,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                BusyTimeEntriesConfirmed = DateTime.MinValue,
                Competency = null,
                KeyQualifications = new List<string>(),
                BusyTimeEntries = new List<EmployeeSearchModel.BusyTime>(),
                DateOfBirthSetManually = false
            };

            return model;
        }

        private static EmployeeSearchModel ConvertTo(ImportedFromCvPartner ev)
        {
            var model = new EmployeeSearchModel
            {
                Id = GetRavenId(ev.EmployeeId),
                GlobalId = ev.EmployeeId,
                CompanyId = ev.CompanyId,
                Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName),
                DateOfBirth = ev.DateOfBirth.HasValue ? ev.DateOfBirth.Value : DateTime.MinValue,
                JobTitle = ev.Title,
                PhoneNumber = ev.Phone,
                Email = ev.Email,
                OfficeName = ev.OfficeName,
                Thumb = CreateThumb(ev.Photo, 80),
                BusyTimeEntriesConfirmed = DateTime.MinValue,
                Competency = CreateCompetency(ev.Technologies),
                KeyQualifications = CreateKeyQalifications(ev.KeyQualifications),
                Descriptions = CreateDescritpions(ev.KeyQualifications),
                BusyTimeEntries = new List<EmployeeSearchModel.BusyTime>(),
                DateOfBirthSetManually = false
            };
            return model;
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, ImportedFromCvPartner ev)
        {
            model.Name = NameService.GetName(ev.FirstName, ev.MiddleName, ev.LastName);
            if (!model.DateOfBirthSetManually)
            {
                model.DateOfBirth = ev.DateOfBirth.HasValue ? ev.DateOfBirth.Value : DateTime.MinValue;
            }
            model.OfficeName = ev.OfficeName;
            model.JobTitle = ev.Title;
            model.PhoneNumber = ev.Phone;
            model.Email = ev.Email;
            model.Thumb = CreateThumb(ev.Photo, 80);
            model.Competency = CreateCompetency(ev.Technologies);
            model.KeyQualifications = CreateKeyQalifications(ev.KeyQualifications);
            model.Descriptions = CreateDescritpions(ev.KeyQualifications);
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

        private EmployeeSearchModel Patch(EmployeeSearchModel model, BusyTimeUpdated ev)
        {
            if (model.BusyTimeEntries != null)
            {
                if (model.BusyTimeEntries.Any(bt => bt.Id == ev.BusyTimeId))
                {
                    var busy = model.BusyTimeEntries.First(bt => bt.Id == ev.BusyTimeId);
                    busy.Start = ev.Start;
                    busy.End = ev.End;
                    busy.PercentageOccupied = ev.PercentageOccpied;
                    busy.Comment = ev.Comment;
                }
            }
            return model;
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, PrivateAddressSet ev)
        {
            model.PrivateAddress = ConvertTo(ev.PrivateAddress);
            return model;
        }

        private static EmployeeSearchModel.Address ConvertTo(Address privateAddress)
        {
            if (privateAddress == null) return null;
            return new EmployeeSearchModel.Address
            {
                Street = privateAddress.Street,
                PostalCode = privateAddress.PostalCode,
                PostalName = privateAddress.PostalName
            };
        }

        private static EmployeeSearchModel Patch(EmployeeSearchModel model, DateOfBirthSet ev)
        {
            model.DateOfBirth = ev.DateOfBirth;
            model.DateOfBirthSetManually = true;
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
                    using (var ms = new MemoryStream())
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

                        byte[] imageBytes = ms.ToArray();
                        var res = "data:image/" + photo.Extension + ";base64," + Convert.ToBase64String(imageBytes);
                        Debug.WriteLine(res);
                        return res;
                    }

                }
            }
        }

    }
}