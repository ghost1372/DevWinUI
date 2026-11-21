// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

/// <summary>
/// Contains all kinds of return status
/// </summary>
public enum ReturnResult : byte
{
	/// <summary>
	/// Informs that operation is still in progress
	/// </summary>
	InProgress = 0,

	/// <summary>
	/// Informs that operation completed successfully
	/// </summary>
	Success = 1,

	/// <summary>
	/// Informs that operation has failed
	/// </summary>
	Failed = 2,

	/// <summary>
	/// Informs that operation has been cancelled
	/// </summary>
	Cancelled = 3,
}
