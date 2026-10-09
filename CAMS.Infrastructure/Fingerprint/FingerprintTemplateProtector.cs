using CAMS.Application.Fingerprint;
using Microsoft.AspNetCore.DataProtection;

namespace CAMS.Infrastructure.Fingerprint;

public sealed class FingerprintTemplateProtector
	: IFingerprintTemplateProtector
{
	private readonly IDataProtector
		_protector;


	public FingerprintTemplateProtector(
		IDataProtectionProvider dataProtectionProvider)
	{
		_protector =
			dataProtectionProvider
				.CreateProtector(
					"CAMS.FingerprintTemplates.v1");
	}


	public byte[] Protect(
		byte[] template)
	{
		ArgumentNullException.ThrowIfNull(
			template);


		return
			_protector.Protect(
				template);
	}


	public byte[] Unprotect(
		byte[] protectedTemplate)
	{
		ArgumentNullException.ThrowIfNull(
			protectedTemplate);


		return
			_protector.Unprotect(
				protectedTemplate);
	}
}