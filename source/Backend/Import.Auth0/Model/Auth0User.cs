using Import.Auth0.Model.Model;
using Newtonsoft.Json;

namespace Import.Auth0.Model
{
    public class Auth0User
    {
        [JsonProperty("_id")]
        public string Auth0Id { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("identities")]
        public Identity[] Identities { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("primaryEmail")]
        public string PrimaryEmail { get; set; }

         

        [JsonProperty("isDelegatedAdmin")]
        public bool IsDelegatedAdmin { get; set; }

        [JsonProperty("lastLoginTime")]
        public string LastLoginTime { get; set; }

        [JsonProperty("creationTime")]
        public string CreationTime { get; set; }

        [JsonProperty("agreedToTerms")]
        public bool AgreedToTerms { get; set; }

        [JsonProperty("suspended")]
        public bool Suspended { get; set; }

        [JsonProperty("changePasswordAtNextLogin")]
        public bool ChangePasswordAtNextLogin { get; set; }

        [JsonProperty("ipWhitelisted")]
        public bool IpWhitelisted { get; set; }

        [JsonProperty("emails")]
        public Email[] Emails { get; set; }

        [JsonProperty("phones")]
        public Phone[] Phones { get; set; }

        [JsonProperty("aliases")]
        public string[] Aliases { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("orgUnitPath")]
        public string OrgUnitPath { get; set; }

        [JsonProperty("isMailboxSetup")]
        public bool IsMailboxSetup { get; set; }

        [JsonProperty("includeInGlobalAddressList")]
        public bool IncludeInGlobalAddressList { get; set; }

        [JsonProperty("thumbnailPhotoUrl")]
        public string ThumbnailPhotoUrl { get; set; }

        [JsonProperty("is_admin")]
        public bool IsMaybeAdmin1 { get; set; }

        [JsonProperty("isAdmin")]
        public bool IsMaybeAdmin2 { get; set; }

        public bool IsProbablyAdminForSomething { get { return IsMaybeAdmin1 || IsMaybeAdmin2; } }

        [JsonProperty("is_suspended")]
        public bool IsSuspended { get; set; }

        [JsonProperty("is_ipWhitelisted")]
        public bool IsIpWhitelisted { get; set; }

        [JsonProperty("tou_accepted")]
        public bool TouAccepted { get; set; }

        [JsonProperty("organizations")]
        public Organization[] Organizations { get; set; }
    }
}