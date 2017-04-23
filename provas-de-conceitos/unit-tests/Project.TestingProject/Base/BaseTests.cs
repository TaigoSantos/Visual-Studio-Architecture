﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Project.TestingProject.Base
{
    [TestClass]
    public class BaseTests
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            Console.WriteLine("AssemblyInit " + context.TestName);

            var dbSetup = new DatabaseSetup();
            dbSetup.WipeAndCreateDatabase();
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");
        }
    }
}
