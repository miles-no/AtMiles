using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using no.miles.at.Backend.Api.Models.Api.Employee;
using no.miles.at.Backend.Api.Models.Api.Tasks;

namespace no.miles.at.Backend.Api.Utilities
{
    public class vCardMediaTypeFormatter : MediaTypeFormatter
    {
        public vCardMediaTypeFormatter()
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/x-vcard"));
        }

        public override bool CanReadType(Type type)
        {
            return (type == typeof(IEnumerable<EmployeeDetailsResponse>));
        }

        public override bool CanWriteType(Type type)
        {
            return (type == typeof(IEnumerable<EmployeeDetailsResponse>));
        }
        

        //protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        //{
        //    return Task.Factory.StartNew(() =>
        //    {
        //        WriteVCard((IEnumerable<Response>)value, stream);
        //    });
        //}

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            return Task.Factory.StartNew(() => WriteVCard((IEnumerable<EmployeeDetailsResponse>)value, writeStream));
        }

        private void WriteVCard(IEnumerable<EmployeeDetailsResponse> contactModels, Stream stream)
        {
            var enumerator = contactModels.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var contactModel = enumerator.Current;
                var buffer = new StringBuilder();
                buffer.AppendLine("BEGIN:VCARD");
                buffer.AppendLine("VERSION:2.1");
                buffer.AppendFormat("N:{0}\r\n", contactModel.Name);
                //TODO: Uncomment this (and remove line above) when given an family name is available
                //buffer.AppendFormat("N:{0};{1}\r\n", contactModel.FirstName, contactModel.GivenName);
                buffer.AppendFormat("EMAIL:{0}\r\n", contactModel.Email);
                buffer.AppendFormat("TEL;TYPE=cell:{0}\r\n", contactModel.PhoneNumber);

                buffer.AppendLine("END:VCARD");

                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(buffer);
                }
            }
        }
    }
}