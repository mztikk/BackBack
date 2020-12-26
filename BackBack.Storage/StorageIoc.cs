using System;
using System.Collections.Generic;
using RF.WPF;
using StyletIoC;

namespace BackBack.Storage
{
    public class StorageIoc : IocBase
    {
        private readonly List<Type> _storageTypes = new List<Type>();

        public override void Configure(IContainer container)
        {
            foreach (Type type in _storageTypes)
            {
                var storage = container.Get(type) as IStorage;
                storage?.ILoad();
            }
        }

        protected override void Setup()
        {
            foreach (Type type in GetAllTypes())
            {
                string? baseName = type.BaseType?.FullName;
                if (baseName?.StartsWith("RF.WPF.Storage`") == true || baseName?.StartsWith("RF.WPF.EncryptedStorage`") == true)
                {
                    Bind(type).ToSelf().InSingletonScope();
                    _storageTypes.Add(type);
                }
            }
        }
    }
}
