using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Contact.Backend.MockStore;
using Contact.Domain.Events.Employee;
using Contact.Infrastructure;

namespace Contact.ReadStore.Test
{
    public class FillReadStore
    {
        public void FillAndPrepare()
        {
            Mapper.CreateMap<EmployeeCreated, PersonSearchModel>()
                .ForMember(dest => dest.Competency, source => 
                    source.MapFrom(s => s.Competence != null ? s.Competence.Select(competenceTag=>new Tag{Category = competenceTag.LocalCategory, Competency = competenceTag.LocalSubject, InternationalCategory = competenceTag.InternationalCategory, InternationalCompentency = competenceTag.InternationalSubject}) : null))
                .ForMember(dest => dest.Id, source => source.MapFrom(s=>s.GlobalId))
                .ForMember(dest => dest.Name, source => source.MapFrom(
                    m => m.FirstName + " " +
                         (string.IsNullOrEmpty(m.MiddleName) ? string.Empty : (m.MiddleName + " ")) + m.LastName))
                .ForMember(dest=>dest.Thumb, source => source.MapFrom<string>(s => CreateThumb(s.Photo, 80, 80)));


            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            const string password = "changeit";

            var handler = new ReadModelHandler();
            handler.RegisterHandler<EmployeeCreated>(FillRaven);
            var demo = new EventStoreDispatcher(host, username, password, handler, new ConsoleLogger(), () => { });
            demo.Start();
            
            Console.ReadLine();
        }

        private static string CreateThumb(Domain.ValueTypes.Picture photo, int width, int height)
        {
            if (photo == null)
            {
                return null;
            }
            Stream stream = new MemoryStream(photo.Content);
            using (var srcImage = Image.FromStream(stream))
            {
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

        private static void FillRaven(EmployeeCreated obj)
        {

            var searchModel = Mapper.Map<EmployeeCreated, PersonSearchModel>(obj);
            using (var session = MockStore.DocumentStore.OpenSession())
            {
                session.Store(searchModel);
                session.SaveChanges();
            }
        }
    }
}