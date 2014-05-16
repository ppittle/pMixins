// Guids.cs
// MUST match guids.h
using System;

namespace Company.pMixins_ItemTemplate_Package
{
    static class GuidList
    {
        public const string guidpMixins_ItemTemplate_PackagePkgString = "f570ab75-84bd-49bc-96a3-16a0363f16c1";
        public const string guidpMixins_ItemTemplate_PackageCmdSetString = "bbc54ffd-2b76-4b2f-8fa7-2bfe61a19e9f";

        public static readonly Guid guidpMixins_ItemTemplate_PackageCmdSet = new Guid(guidpMixins_ItemTemplate_PackageCmdSetString);
    };
}