﻿#region License
// Copyright (c) 2014 Bit Rave Pty Ltd
//
// 1. OWNERSHIP, LICENSE GRANT
// Subject to the terms below (the 'License Agreement'), Bit Rave Pty Ltd ('We', 'Us') 
// grants you to install and use Azure Mobile Services for Unity (the 'Software').
//
// This is a license agreement and not an agreement for sale. We reserve ownership 
// of all intellectual property rights inherent in or relating to the Software, 
// which include, but are not limited to, all copyright, patent rights, all rights 
// in relation to registered and unregistered trademarks (including service marks), 
// confidential information (including trade secrets and know-how) and all rights 
// other than those expressly granted by this Agreement.
//
// Subject to the terms and conditions of this License Agreement, We grant to You 
// a non-transferable, non-exclusive license for a Designated User (as defined below) 
// within Your organization to install and use the Software on any workstations used 
// exclusively by such Designated User and for You to distribute the Software as part 
// of your Unity applications or games, solely in connection with distribution of 
// the Software in accordance with sections 3 and 4 below. This license is not 
// sublicensable except as explicitly set forth herein. "Designated User(s)" shall 
// mean Your employee(s) acting within the scope of their employment or Your consultant(s) 
// or contractor(s) acting within the scope of the services they provide for You or on Your behalf.

// 2. PERMITTED USES, SOURCE CODE, MODIFICATIONS
// We provide You with source code so that You can create Modifications of the original Software, 
// where Modification means: a) any addition to or deletion from the contents of a file included 
// in the original Software or previous Modifications created by You, or b) any new file that 
// contains any part of the original Software or previous Modifications. While You retain all 
// rights to any original work authored by You as part of the Modifications, We continue to own 
// all copyright and other intellectual property rights in the Software.

// 3. DISTRIBUTION
// You may distribute the Software in any applications, frameworks, or elements (collectively 
// referred to as "Applications") that you develop using the Software in accordance with this 
// License Agreement, provided that such distribution does not violate the restrictions set 
// forth in section 4 of this agreement.

// You will not owe Us any royalties for Your distribution of the Software in accordance with 
// this License Agreement.

// 4. PROHIBITED USES
// You may not redistribute the Software or Modifications other than by including the Software 
// or a portion thereof within Your own product, which must have substantially different 
// functionality than the Software or Modifications and must not allow any third party to use 
// the Software or Modifications, or any portions thereof, for software development or application 
// development purposes. You are explicitly not allowed to redistribute the Software or 
// Modifications as part of any product that can be described as a development toolkit or library 
// or is intended for use by software developers or application developers and not end-users.

// 5. TERMINATION
// This Agreement shall terminate automatically if you fail to comply with the limitations 
// described in this Agreement. No notice shall be required to effectuate such termination. 
// Upon termination, you must remove and destroy all copies of the Software. 

// 6. DISCLAIMER OF WARRANTY
// YOU AGREE THAT WE HAVE MADE NO EXPRESS WARRANTIES, ORAL OR WRITTEN, TO YOU REGARDING THE 
// SOFTWARE AND THAT THE SOFTWARE IS BEING PROVIDED TO YOU 'AS IS' WITHOUT WARRANTY OF ANY KIND.
//  WE DISCLAIM ANY AND ALL OTHER WARRANTIES, WHETHER EXPRESSED, IMPLIED, OR STATUTORY. YOUR RIGHTS
//  MAY VARY DEPENDING ON THE STATE IN WHICH YOU LIVE. WE SHALL NOT BE LIABLE FOR INDIRECT, 
// INCIDENTAL, SPECIAL, COVER, RELIANCE, OR CONSEQUENTIAL DAMAGES RESULTING FROM THE USE OF THIS PRODUCT.

// 7. LIMITATION OF LIABILITY
// YOU USE THIS PROGRAM SOLELY AT YOUR OWN RISK. IN NO EVENT SHALL WE BE LIABLE TO YOU FOR ANY DAMAGES,
// INCLUDING BUT NOT LIMITED TO ANY LOSS, OR OTHER INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF 
// ANY KIND ARISING OUT OF THE USE OF THE SOFTWARE, EVEN IF WE HAVE BEEN ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGES. IN NO EVENT WILL WE BE LIABLE FOR ANY CLAIM, WHETHER IN CONTRACT, TORT, OR ANY OTHER
// THEORY OF LIABILITY, EXCEED THE COST OF THE SOFTWARE. THIS LIMITATION SHALL APPLY TO CLAIMS OF 
// PERSONAL INJURY TO THE EXTENT PERMITTED BY LAW.

// 8. MISCELLANEOUS
// The license granted herein applies only to the version of the Software available when acquired
// in connection with the terms of this Agreement. Any previous or subsequent license granted to
// You for use of the Software shall be governed by the terms and conditions of the agreement entered
// in connection with the acquisition of that version of the Software. You agree that you will comply
// with all applicable laws and regulations with respect to the Software, including without limitation
// all export and re-export control laws and regulations.
#endregion


using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Bitrave.Azure
{
    /// <summary>
    /// Why does this factory exist?  To get access to the azure mobile services, and to 
    /// get around dependency pain in Unity.  The factory uses reflection to remove the dependencies
    /// on some Azure libraries that cause Unity builds to fail for Windows Phone.
    /// </summary>
    //public class AzureMobileServicesFactory
    //{
    //    public static IAzureMobileServices Create(string url, string token)
    //    {
    //        var assembly = System.Reflection.Assembly.Load(new AssemblyName("Bitrave.Azure"));
    //        Type t = assembly.GetType("AzureMobileServices");
    //        var azure = (IAzureMobileServices)Activator.CreateInstance(t);

    //        azure.Initialise(url, token);
    //        return azure;
    //    }
    //}

    // THese are copied from their Azure counterparts to give the same structure

    // Summary:
    //     Authentication providers supported by Mobile Services.
    public enum AuthenticationProvider
    {
        // Summary:
        //     Microsoft Account authentication provider.
        MicrosoftAccount = 0,
        //
        // Summary:
        //     Google authentication provider.
        Google = 1,
        //
        // Summary:
        //     Twitter authentication provider.
        Twitter = 2,
        //
        // Summary:
        //     Facebook authentication provider.
        Facebook = 3,
    }


    public enum AzureResponseStatus
    {
        Success,
        Failure
    }
    public class AzureResponse<T>
    {
        public AzureResponse(AzureResponseStatus status, T responseData)
        {
            Status = status;
            StatusCode = HttpStatusCode.OK;
            StatusDescription = "200 OK";
            ResponseData = responseData;
        }
        public AzureResponse(IRestResponse response)
        {
            Error = response.ErrorMessage;
            SetStatus(response);
            Status = AzureResponseStatus.Failure;
        }

        private void SetStatus(IRestResponse response)
        {
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
        }

        public AzureResponse(Exception exception)
        {
            StatusCode = HttpStatusCode.BadRequest;
            StatusDescription = "An Exception was thrown.  Check \"Error\" property for description.";
            Error = exception.Message + "\n" + exception.StackTrace;
            Status = AzureResponseStatus.Failure;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public String StatusDescription { get; private set; }
        public AzureResponseStatus Status { get; private set; }
        public String Error { get; private set; }
        public T ResponseData { get; internal set; }
        public RestRequestAsyncHandle handle { get; internal set; }
    }
    
    // A copy of the structure so we can use it from Unity
    public class MobileServiceUser
    {
        public string AuthenticationToken { get; set; }
        public string UserId { get; set; }
    }
}
