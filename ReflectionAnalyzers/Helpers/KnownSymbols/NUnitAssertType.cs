namespace ReflectionAnalyzers;

using Gu.Roslyn.AnalyzerExtensions;

internal class NUnitAssertType : QualifiedType
{
    internal readonly QualifiedMethod That;

    internal NUnitAssertType()
        : base("NUnit.Framework.Assert")
    {
        this.That = new QualifiedMethod(this, nameof(this.That));
    }
}
