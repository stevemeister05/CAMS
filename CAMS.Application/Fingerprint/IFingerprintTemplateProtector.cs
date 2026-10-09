namespace CAMS.Application.Fingerprint;

public interface IFingerprintTemplateProtector
{
	byte[] Protect(
		byte[] template);


	byte[] Unprotect(
		byte[] protectedTemplate);
}