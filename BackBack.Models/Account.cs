using System;
using System.Collections.Generic;

namespace BackBack.Models
{
    public class Account : IEquatable<Account>, IEqualityComparer<Account>
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        #region Equality
        public bool Equals(Account other) => Name == other.Name;
        public bool Equals(Account x, Account y) => x.Name == y.Name;
        public int GetHashCode(Account obj) => obj.Name.GetHashCode();
        public override int GetHashCode() => Name.GetHashCode();
        public override bool Equals(object obj) => obj is Account account && account.Name == Name;
        #endregion Equality
    }
}
