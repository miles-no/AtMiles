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
        Random random = new Random(6846);
          
        public void FillAndPrepare()
        {
         
            Mapper.CreateMap<EmployeeCreated, PersonSearchModel>()
                .ForMember(dest => dest.Competency, source => source.MapFrom(s => CreateRandomCompetency(2)))
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


        private List<Tag> CreateRandomCompetency(int howMany)
        {
            List<Tag> randomCompetency = new List<Tag>
            {
                new Tag{Category = "Development", Competency = "C#"},
                new Tag{Category = "Development", Competency = "Java"},
                new Tag{Category = "Development", Competency = ".Net"},
                new Tag{Category = "Development", Competency = "Php"},
                new Tag{Category = "Management", Competency = "Test leader"},
                new Tag{Category = "Management", Competency = "Project leader"},
                new Tag{Category = "Design", Competency = "Ux"},
                new Tag{Category = "Design", Competency = "Design"},
                new Tag{Category = "Design", Competency = "Fancy buttons"},
                new Tag{Category = "Counseling", Competency = "Business analyst"},
            };

            var res = new List<Tag>();

            for (int i = 0; i < howMany; i++)
            {
                res.Add(randomCompetency[this.random.Next() % (randomCompetency.Count - 1)]);
            }

            return res.Distinct().ToList();
        }

        private static string CreateThumb(Domain.ValueTypes.Picture photo, int width, int height)
        {
            if (photo == null)
            {
                return null;
            }
            Stream stream = new MemoryStream(photo.Content);
            Stream outStream = new MemoryStream();
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
                        //dpgraphic.image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
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