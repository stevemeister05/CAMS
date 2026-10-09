namespace CAMS.Domain.Entities;

public class MemberFingerprint
	: BaseEntity
{
	public Guid MemberId
	{
		get;
		set;
	}


	/*
	 * Store the protected/encrypted template,
	 * not the plain Futronic template.
	 */
	public byte[] ProtectedTemplate
	{
		get;
		set;
	} = [];


	public string? FingerLabel
	{
		get;
		set;
	}


	public bool IsActive
	{
		get;
		set;
	} =
		true;


	public Member Member
	{
		get;
		set;
	} = null!;
}