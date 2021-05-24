// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;

namespace IronButterflyBot
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserProfile
    {
        public string IouOption { get; set; }

        public string PaymentAmount { get; set; }

        public string PaymentMethod { get; set; }

        public string PhoneNumber { get; set; }

        public string AppType { get; set; }
    }
}
