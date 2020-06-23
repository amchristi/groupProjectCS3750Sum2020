using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace MackTechGroupProject.Extensions
{
    public static class IdentityExtensions
    {
        
        public static string GetUserFirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserLastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastName");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserAddressOne(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AddressOne");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserAddressTwo(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("AddressTwo");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserCity(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("City");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserState(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("State");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserZipCode(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ZipCode");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserPhoneNumber(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("PhoneNumber");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserLinkOne(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LinkOne");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserLinkTwo(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LinkTwo");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserLinkThree(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LinkThree");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetUserBioInfo(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("BioInfo");

            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}