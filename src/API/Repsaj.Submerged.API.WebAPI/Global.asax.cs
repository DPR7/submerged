﻿using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Repsaj.Submerged.API
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            ControllerBuilder.Current.DefaultNamespaces.Add("Microsoft.Azure.Devices.Applications.RemoteMonitoring.DeviceAdmin.Web.WebApiControllers");

            // possible Fix for SignalR (https://github.com/SignalR/SignalR/issues/3148)
            GlobalHost.Configuration.TransportConnectTimeout = TimeSpan.FromSeconds(10);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        // Require HTTPS for all requests processed by ASP.NET
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = GetSelectedCulture();

            if (Context.Request.IsSecureConnection)
            {
                // HSTS blocks access to sites with invalid certs
                bool usingValidTlsCert = false;

                // tell the browser that this site is ALWAYS https (but only if the cert is valid!)
                if (usingValidTlsCert)
                {
                    // note: to clear this from a browser, set the header with "max-age=0"
                    Response.AddHeader("Strict-Transport-Security", "max-age=3600");
                }
            }
            else
            {
                // (if we are serving HTTP) redirect users to HTTPS
                Response.RedirectPermanent(Context.Request.Url.ToString().Replace("http://", "https://"), false);
                CompleteRequest();
            }
        }

        protected void Application_EndRequest()
        {
            // This implementation is a work around for the HTTP 302 issue
            // with MVC and ajax requests. The code below modifies the status code
            // to 401 if the status is 302 and the request is ajax based.
            var context = new HttpContextWrapper(Context);
            if (context.Response.StatusCode == 302 && GetIsServiceCall(context))
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
            }
        }

        private CultureInfo GetSelectedCulture()
        {
            // Add custom logic here for getting a user-specified culture.

            // Default to server-selected one.
            return CultureInfo.CurrentCulture;
        }
        private bool GetIsServiceCall(HttpContextWrapper contextWrapper)
        {
            Debug.Assert(contextWrapper != null, "contextWrapper is a null reference.");

            if (contextWrapper.Request.IsAjaxRequest())
            {
                return true;
            }

            string apiPath = VirtualPathUtility.ToAbsolute("~/api/");
            return contextWrapper.Request.Url.LocalPath.StartsWith(apiPath, StringComparison.Ordinal);
        }
    }
}