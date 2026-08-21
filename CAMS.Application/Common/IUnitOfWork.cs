using System;
using System.Collections.Generic;
using System.Text;

namespace CAMS.Application.Common;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync(
		CancellationToken cancellationToken = default);

	Task<IUnitOfWorkTransaction> BeginTransactionAsync(
		CancellationToken cancellationToken = default);
}