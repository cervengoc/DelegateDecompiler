﻿// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup05BasicFeatures
{
    class Test01Select_WithContextExtensionMethod
    {
        private ClassEnvironment classEnv;


        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestSubqueryAsContextExtensionMethod()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => new ParentIdWithFirstChildId()
                {
                    ParentId = x.EfParentId,
                    FirstChildId = env.Db.EfChildren.Where(e => e.EfParentId == x.EfParentId).Select(e => e.EfChildId).FirstOrDefault()
                }).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => new ParentIdWithFirstChildId()
                {
                    ParentId = x.EfParentId,
                    FirstChildId = env.Db.GetFirstChildIdByParent(x.EfParentId)
                }).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        class ParentIdWithFirstChildId
        {
            public int ParentId { get; set; }
            public int FirstChildId { get; set; }
        }
    }
}