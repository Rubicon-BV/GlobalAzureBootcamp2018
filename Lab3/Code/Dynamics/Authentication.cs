// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================
//<snippetAuthentication>

using Microsoft.IdentityModel.Clients.ActiveDirectory;
namespace SimpleEchoBot.Dynamics
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// Manages user authentication with the Dynamics CRM Web API (OData v4) services. This class uses Microsoft Azure
    /// Active Directory Authentication Library (ADAL) to handle the OAuth 2.0 protocol. 
    /// </summary>
    public class Authentication
    {
        private Config _config = null;
        private HttpMessageHandler _clientHandler = null;
        private AuthenticationContext _context = null;
        private string _authority = null;

        #region Constructors
        /// <summary>
        /// Establishes an authentication session for the service.
        /// </summary>
        /// <param name="config">A populated configuration object.</param>
        public Authentication(Config config)
        {
            if (config == null)
                throw new Exception("Configuration cannot be null.");

            _config = config;

            _clientHandler = new OAuthMessageHandler(this, new HttpClientHandler());
            _context = new AuthenticationContext(Authority, false);

        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// The authentication context.
        /// </summary>
        public AuthenticationContext Context
        { get { return _context; } }

        /// <summary>
        /// The HTTP client message handler.
        /// </summary>
        public HttpMessageHandler ClientHandler
        { get { return _clientHandler; } }

        /// <summary>
        /// The URL of the authority to be used for authentication.
        /// </summary>
        public string Authority
        {
            get
            {
                if (_authority == null)
                    _authority = DiscoverAuthority(_config.ResourceUrl);

                return _authority;
            }

            set { _authority = value; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Returns the authentication result for the configured authentication context.
        /// </summary>
        /// <returns>The refreshed access token.</returns>
        /// <remarks>Refresh the access token before every service call to avoid having to manage token expiration.</remarks>
        public AuthenticationResult AcquireToken()
        {
           
            var clientcred = new ClientCredential(_config.AppId, _config.SecretId);
            var authenticationContext = new AuthenticationContext(Authority);
            return authenticationContext.AcquireTokenAsync(_config.ResourceUrl, clientcred).Result;
        }
        
        /// <summary>
        /// Discover the authentication authority.
        /// </summary>
        /// <returns>The URL of the authentication authority on the specified endpoint address, or an empty string
        /// if the authority cannot be discovered.</returns>
         public static string DiscoverAuthority(string serviceUrl)
        {
            try
            {
                AuthenticationParameters ap = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(serviceUrl + "/api/data/")).Result;

                return ap.Authority;
            }
            catch (HttpRequestException e)
            {
                throw new Exception("An HTTP request exception occurred during authority discovery.", e);
            }
            catch (System.Exception e )
            {
                // This exception ocurrs when the service is not configured for OAuth.
                if( e.HResult == -2146233088 )
                {
                    return String.Empty;
                }
                else
                {
                    throw e;
                }
            }
        }
        #endregion Methods

        /// <summary>
        /// Custom HTTP client handler that adds the Authorization header to message requests. This
        /// is required for IFD and Online deployments.
        /// </summary>
        class OAuthMessageHandler : DelegatingHandler
        {
            Authentication _auth = null;

            public OAuthMessageHandler( Authentication auth, HttpMessageHandler innerHandler )
                : base(innerHandler)
            {
                _auth = auth;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                // It is a best practice to refresh the access token before every message request is sent. Doing so
                // avoids having to check the expiration date/time of the token. This operation is quick.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _auth.AcquireToken().AccessToken);

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
//</snippetAuthentication>
