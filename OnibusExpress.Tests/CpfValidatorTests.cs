using OnibusExpress.Services;

namespace OnibusExpress.Tests;

public sealed class CpfValidatorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void IsValid_DeveAceitarCpfComDigitoVerificadorValido(string cpf)
    {
        Assert.True(CpfValidator.IsValid(cpf));
    }

    [Theory]
    [InlineData("529.982.247-26")]
    [InlineData("111.111.111-11")]
    [InlineData("5299822472")]
    [InlineData("529982247250")]
    [InlineData("529.982.24725")]
    public void IsValid_DeveRejeitarCpfInvalidoOuComFormatoInvalido(string cpf)
    {
        Assert.False(CpfValidator.IsValid(cpf));
    }

    [Fact]
    public void Format_DeveSalvarCpfNoFormatoLegivel()
    {
        var formatted = CpfValidator.Format("52998224725");

        Assert.Equal("529.982.247-25", formatted);
    }
}
