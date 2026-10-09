using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Domain.Entities;

public class SystemSetting
{
	public SystemSetting()
	{
		this.Id = Guid.NewGuid();
	}

	public Guid Id { get; set; }

	public string Key { get; set; } = string.Empty;

	public string Value { get; set; } = string.Empty;

	public string? Description { get; set; }

	public DateTime UpdatedAt { get; set; }
}
