namespace OlimpBack.Application.Permissions;

/// <summary>
/// Lightweight role data for hierarchy checks (from DB flags + name fallback).
/// </summary>
public readonly record struct RoleKindSnapshot(bool IsStudent, bool IsAdmin, string Name);
