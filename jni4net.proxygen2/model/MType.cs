using System;
using System.Collections.Generic;
using java.lang;
using net.sf.jni4net.proxygen.config;
using net.sf.jni4net.proxygen.visitors;

namespace net.sf.jni4net.proxygen.model
{
    public class MType : IModel
    {
        public CType Config { get; set; }

        public Type Clr { get; set; }
        public Class Jvm { get; set; }

        //short class name
        public string Name { get; set; }

        // low, uniform, dots, incl enclosing types
        public string NameSpace { get; set; }

        public bool IsPrimitive { get; set; }
        public bool IsException { get; set; }
        public bool IsDelegate { get; set; }
        public bool IsOut { get; set; }
        public bool IsRef { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsArray { get; set; }
        public bool IsFinal { get; set; }
        public bool IsNested { get; set; }
        public bool IsInterface { get; set; }

        public bool IsRootType { get; set; }
        public bool IsKnown { get; set; }
        public bool IsGenClr { get; set; }
        public bool IsGenJvm { get; set; }
        public MType Subst { get; set; }

        public MType Enclosing { get; set; }
        public MType Base;
        public MType Element;
        public List<MType> NextedTypes = new List<MType>();

        // just immediate interfaces, not inherited from base
        public List<MType> Interfaces = new List<MType>();

        // just immediate methods, not inherited from base
        public List<MMember> Methods=new List<MMember>();
        public List<MMember> SkippedMethods = new List<MMember>();
        public List<MMember> Constructors = new List<MMember>();

        // static information
        public CGType GStaticClr { get; set; }
        public JGType GStaticJvm { get; set; }

        // interface or public type
        public CGType GFaceClr { get; set; }
        public JGType GFaceJvm { get; set; }

        // instance
        public CGType GProxyClr { get; set; }
        public JGType GProxyJvm { get; set; }

        // both implemented on CLR side
        public CGType GImplementation { get; set; }

        public List<GFile> GFiles =new List<GFile>();

        public MType(Type type)
        {
            Config=new CType();
            Clr = type;
            Name = type.Name;

            string fullName = type.FullName.Replace('+', '.');
            int ld = fullName.LastIndexOf('.');
            NameSpace = ld == -1
                            ? ""
                            : fullName.Substring(0, ld);
        }

        public MType(Class clazz)
        {
            Jvm = clazz;
            Name = clazz.getSimpleName();
            string fullName = ((string)clazz.getName()).Replace('$','.');
            int ld = fullName.LastIndexOf('.');
            NameSpace = ld == -1
                            ? ""
                            : fullName.Substring(0, ld);
        }

        public void Accept(IModelVisitor visitor, Repository repository)
        {
            visitor.VisitType(this, repository);
            //the collection is changing
            List<MMember> copyMembers = new List<MMember>(Methods);
            foreach (MMember member in copyMembers)
            {
                member.Accept(visitor, repository);
            }
            foreach (GFile file in GFiles)
            {
                file.Accept(visitor, repository);
            }
        }

        public string Key
        {
            get { return (NameSpace + "." + Name).ToLowerInvariant();}
        }

        public override string ToString()
        {
            return NameSpace + "." + Name;
        }

        public override int GetHashCode()
        {
            return NameSpace.GetHashCode() ^ Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            MType o = obj as MType;
            if (o==null)
                return false;
            return NameSpace.Equals(o.NameSpace)
                && Name.Equals(o.Name);
        }
    }
}