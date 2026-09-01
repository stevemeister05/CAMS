using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Event;

public interface IEventStatusService
{
	Task UpdateStatusesAsync(
		CancellationToken cancellationToken = default);
}
