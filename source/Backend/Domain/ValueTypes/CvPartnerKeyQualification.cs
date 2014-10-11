namespace Contact.Domain.ValueTypes
{
    public class CvPartnerKeyQualification
    {
        public readonly string InternationalDescription;
        public readonly string LocalDescription;
        public readonly CvPartnerKeyPoint[] Keypoints;

        public CvPartnerKeyQualification(string internationalDescription, string localDescription, CvPartnerKeyPoint[] keypoints)
        {
            InternationalDescription = internationalDescription;
            LocalDescription = localDescription;
            Keypoints = keypoints;
        }
    }
}
