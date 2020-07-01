using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MackTechGroupProject.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressOne { get; set; }
        public string AddressTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string LinkOne { get; set; }
        public string LinkTwo { get; set; }
        public string LinkThree { get; set; }
        public string BioInfo { get; set; }
        public byte[] ProfileImage { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", this.FirstName));
            userIdentity.AddClaim(new Claim("LastName", this.LastName));
            userIdentity.AddClaim(new Claim("AddressOne", this.AddressOne));
            userIdentity.AddClaim(new Claim("AddressTwo", this.AddressTwo));
            userIdentity.AddClaim(new Claim("City", this.City));
            userIdentity.AddClaim(new Claim("State", this.State));
            userIdentity.AddClaim(new Claim("ZipCode", this.ZipCode));
            userIdentity.AddClaim(new Claim("PhoneNumber", this.PhoneNumber));
            userIdentity.AddClaim(new Claim("LinkOne", this.LinkOne));
            userIdentity.AddClaim(new Claim("LinkTwo", this.LinkTwo));
            userIdentity.AddClaim(new Claim("LinkThree", this.LinkThree));
            userIdentity.AddClaim(new Claim("BioInfo", this.BioInfo));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("TitanDbConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}