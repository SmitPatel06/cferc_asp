using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;

/* 
 * Password Hashing With PBKDF2 (http://crackstation.net/hashing-security.htm).
 * Copyright (c) 2013, Taylor Hornby
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice, 
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 */

namespace cferc_website.Repository
{
    public class security
    {
        private const int SALT_SIZE = 32;
        private const int HASH_SIZE = 32;
        private const int ITERATIONS = 1000;
        private const int ITER_INDEX = 0;
        private const int SALT_INDEX = 1;
        private const int PBKDF2_INDEX = 2;

        public static string createHash(string pPassword)
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_SIZE];
            csprng.GetBytes(salt);

            byte[] hash = PBKDF2(pPassword, salt, ITERATIONS, HASH_SIZE);
            return ITERATIONS + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);

            
        }

        public static bool validatePassword(string pPassword, string pCorrectHash)
        {
            char[] delimiter = { ':' };
            string[] split = pCorrectHash.Split(delimiter);
            int iterations = Int32.Parse(split[ITER_INDEX]);
            byte[] salt = Convert.FromBase64String(split[SALT_INDEX]);
            byte[] hash = Convert.FromBase64String(split[PBKDF2_INDEX]);

            byte[] testHash = PBKDF2(pPassword, salt, iterations, hash.Length);

            return SlowEquals(hash, testHash);
        }

        private static byte[] PBKDF2(string pPassword, byte[] pSalt, int pIterations, int pOutput)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pPassword, pSalt);
            pbkdf2.IterationCount = pIterations;
            return pbkdf2.GetBytes(pOutput);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }


    }

    public class userModelToSend
    {
        int selectedUserID { get; set; }
        string userName { get; set; }
        
        
    
    }

    public class role
    {
        public int roleID { get; set; }
        public string roleName { get; set; }
        public role(int pRoleID, string pRoleName)
        {
            this.roleID = pRoleID;
            this.roleName = pRoleName;
        }
    }

    public class adminViewModel
    {
        public List<role> roles { get; set; }
        public int selectedRoleID { get; set; }

        public IEnumerable<SelectListItem> roleItems
        {
            get { return new SelectList(roles, "roleID", "roleName"); }
        }

        public string userName { get; set; }
        public string userPasswordFirst { get; set; }
        public string userPasswordSecond { get; set; }

        public adminViewModel(List<role> pRoles)
        {
            this.roles = pRoles;
        }

        public adminViewModel()
        {

        }
    }
}