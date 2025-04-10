using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{

    public class CandidateNamingService 
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string GenerateCandidateIdentifier(DatabaseTable originalTable)
            => GenerateCandidateIdentifier(originalTable.Name);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string GenerateCandidateIdentifier(DatabaseColumn originalColumn)
            => GenerateCandidateIdentifier(originalColumn.Name);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string GetDependentEndCandidateNavigationPropertyName(IReadOnlyForeignKey foreignKey)
        {
            var candidateName = FindCandidateNavigationName(foreignKey.Properties);

            return !string.IsNullOrEmpty(candidateName) ? candidateName : foreignKey.PrincipalEntityType.ShortName();
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string GetPrincipalEndCandidateNavigationPropertyName(
            IReadOnlyForeignKey foreignKey,
            string dependentEndNavigationPropertyName)
        {
            var allForeignKeysBetweenDependentAndPrincipal =
                foreignKey.PrincipalEntityType.GetReferencingForeignKeys()
                    .Where(fk => foreignKey.DeclaringEntityType == fk.DeclaringEntityType);

            return allForeignKeysBetweenDependentAndPrincipal?.Count() > 1
                ? foreignKey.DeclaringEntityType.ShortName()
                + dependentEndNavigationPropertyName
                : foreignKey.DeclaringEntityType.ShortName();
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string GenerateCandidateIdentifier(string originalIdentifier)
        {
            var candidateStringBuilder = new StringBuilder();
            var previousLetterCharInWordIsLowerCase = false;
            var isFirstCharacterInWord = true;
            foreach (var c in originalIdentifier)
            {
                var isNotLetterOrDigit = !char.IsLetterOrDigit(c);
                if (isNotLetterOrDigit
                    || (previousLetterCharInWordIsLowerCase && char.IsUpper(c)))
                {
                    isFirstCharacterInWord = true;
                    previousLetterCharInWordIsLowerCase = false;
                    if (isNotLetterOrDigit)
                    {
                        continue;
                    }
                }

                candidateStringBuilder.Append(
                    isFirstCharacterInWord ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
                isFirstCharacterInWord = false;
                if (char.IsLower(c))
                {
                    previousLetterCharInWordIsLowerCase = true;
                }
            }

            return candidateStringBuilder.ToString();
        }

        private static string FindCandidateNavigationName(IEnumerable<IReadOnlyProperty> properties)
        {
            var name = "";
            foreach (var property in properties)
            {
                if (name != "")
                {
                    return "";
                }

                name = property.Name;
            }

            return StripId(name);
        }

        private static string StripId(string commonPrefix)
        {
            if (commonPrefix.Length < 3
                || !commonPrefix.EndsWith("id", StringComparison.OrdinalIgnoreCase))
            {
                return commonPrefix;
            }

            var ignoredCharacterCount = 2;
            if (commonPrefix.Length > 4
                && commonPrefix.EndsWith("guid", StringComparison.OrdinalIgnoreCase))
            {
                ignoredCharacterCount = 4;
            }

            int i;
            for (i = commonPrefix.Length - ignoredCharacterCount - 1; i >= 0; i--)
            {
                if (char.IsLetterOrDigit(commonPrefix[i]))
                {
                    break;
                }
            }

            return i != 0
                ? commonPrefix[..(i + 1)]
                : commonPrefix;
        }
    }


    public interface ICSharpUtilities
    {
        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        string GenerateCSharpIdentifier(
            string identifier,
            ICollection<string>? existingIdentifiers,
            Func<string, string>? singularizePluralizer);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        string GenerateCSharpIdentifier(
            string identifier,
            ICollection<string>? existingIdentifiers,
            Func<string, string>? singularizePluralizer,
            Func<string, ICollection<string>?, string> uniquifier);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        bool IsCSharpKeyword(string identifier);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        bool IsValidIdentifier(string? name);
    }


    public class CSharpNamer<T>
    where T : notnull
    {
        private readonly Func<T, string> _nameGetter;
        private readonly ICSharpUtilities _cSharpUtilities;
        private readonly Func<string, string>? _singularizePluralizer;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected readonly Dictionary<T, string> NameCache = new();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public CSharpNamer(
            Func<T, string> nameGetter,
            ICSharpUtilities cSharpUtilities,
            Func<string, string>? singularizePluralizer)
        {
            _nameGetter = nameGetter;
            _cSharpUtilities = cSharpUtilities;
            _singularizePluralizer = singularizePluralizer;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string GetName(T item)
        {
            if (NameCache.TryGetValue(item, out var cachedName))
            {
                return cachedName;
            }

            var name = _cSharpUtilities.GenerateCSharpIdentifier(
                _nameGetter(item), existingIdentifiers: null, singularizePluralizer: _singularizePluralizer);
            NameCache.Add(item, name);
            return name;
        }
    }

    public class CSharpUniqueNamer<T> : CSharpNamer<T>
     where T : notnull
    {
        private readonly HashSet<string> _usedNames;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public CSharpUniqueNamer(
            Func<T, string> nameGetter,
            ICSharpUtilities cSharpUtilities,
            Func<string, string>? singularizePluralizer,
            bool caseSensitive)
            : this(nameGetter, null, cSharpUtilities, singularizePluralizer, caseSensitive)
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public CSharpUniqueNamer(
            Func<T, string> nameGetter,
            IEnumerable<string>? usedNames,
            ICSharpUtilities cSharpUtilities,
            Func<string, string>? singularizePluralizer,
            bool caseSensitive)
            : base(nameGetter, cSharpUtilities, singularizePluralizer)
        {
            _usedNames = new HashSet<string>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            if (usedNames != null)
            {
                foreach (var name in usedNames)
                {
                    _usedNames.Add(name);
                }
            }
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public override string GetName(T item)
        {
            if (NameCache.ContainsKey(item))
            {
                return base.GetName(item);
            }

            var input = base.GetName(item);
            var name = input;
            var suffix = 1;

            while (_usedNames.Contains(name))
            {
                name = input + suffix++;
            }

            _usedNames.Add(name);
            NameCache[item] = name;

            return name;
        }
    }
}
