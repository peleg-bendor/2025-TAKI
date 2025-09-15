using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

namespace TakiGame {
	/// <summary>
	/// Centralized logging system for TAKI game with categorized, level-controlled output
	/// Replaces scattered Debug.Log calls with organized, configurable logging
	/// </summary>
	public static class TakiLogger {

		// Log levels for controlling verbosity (standard hierarchy)
		public enum LogLevel {
			Off = 0,        // No logging
			Error = 1,      // Only errors
			Warning = 2,    // Errors + warnings  
			Info = 3,       // Errors + warnings + info
			Debug = 4,      // Errors + warnings + info + debug
			Trace = 5       // Everything including trace details
		}

		// Log categories for different systems
		public enum LogCategory {
			TurnFlow,       // Strict turn flow system
			CardPlay,       // Card playing and drawing
			GameState,      // Game state changes
			TurnManagement, // Turn switching and timing
			UI,             // UI updates and user interaction
			AI,             // Computer AI decisions
			Deck,           // Deck operations
			Rules,          // Rule validation
			System,         // System integration and events
			Diagnostics,    // Debug and diagnostic information
			Investigation,  // Personal Investigation
			SpecialCards,   // PHASE 7: Special card effects and validation
			Network,
			Multiplayer
		}

		// Current log level - can be changed at runtime
		public static LogLevel currentLogLevel = LogLevel.Info;

		// Production mode toggle - minimal logging for release
		public static bool isProductionMode = false;

		// Per-category log levels (hardcoded configuration)
		private static readonly Dictionary<LogCategory, LogLevel> categoryLogLevels = new Dictionary<LogCategory, LogLevel> {
			{ LogCategory.TurnFlow, LogLevel.Debug },
			{ LogCategory.CardPlay, LogLevel.Debug },
			{ LogCategory.GameState, LogLevel.Debug },
			{ LogCategory.TurnManagement, LogLevel.Debug },
			{ LogCategory.UI, LogLevel.Debug },
			{ LogCategory.AI, LogLevel.Debug },
			{ LogCategory.Deck, LogLevel.Debug },
			{ LogCategory.Rules, LogLevel.Debug },
			{ LogCategory.System, LogLevel.Debug },
			{ LogCategory.Diagnostics, LogLevel.Trace },
			{ LogCategory.Investigation, LogLevel.Trace },
			{ LogCategory.SpecialCards, LogLevel.Debug },
			{ LogCategory.Network, LogLevel.Debug },
			{ LogCategory.Multiplayer, LogLevel.Debug }
		};

		// Category-specific logging methods

		/// <summary>
		/// Log turn flow related messages (strict turn system)
		/// </summary>
		public static void LogTurnFlow (string message, LogLevel level = LogLevel.Debug) {
			Log (LogCategory.TurnFlow, message, level);
		}

		/// <summary>
		/// Log card play and draw operations
		/// </summary>
		public static void LogCardPlay (string message, LogLevel level = LogLevel.Info) {
			Log (LogCategory.CardPlay, message, level);
		}

		/// <summary>
		/// Log game state changes and transitions
		/// </summary>
		public static void LogGameState (string message, LogLevel level = LogLevel.Info) {
			Log (LogCategory.GameState, message, level);
		}

		/// <summary>
		/// Log turn management (switching, timing, player changes)
		/// </summary>
		public static void LogTurnManagement (string message, LogLevel level = LogLevel.Debug) {
			Log (LogCategory.TurnManagement, message, level);
		}

		/// <summary>
		/// Log UI updates and user interactions
		/// </summary>
		public static void LogUI (string message, LogLevel level = LogLevel.Debug) {
			Log (LogCategory.UI, message, level);
		}

		/// <summary>
		/// Log AI decisions and computer player actions
		/// </summary>
		public static void LogAI (string message, LogLevel level = LogLevel.Info) {
			Log (LogCategory.AI, message, level);
		}

		/// <summary>
		/// Log deck operations (draw, discard, shuffle)
		/// </summary>
		public static void LogDeck (string message, LogLevel level = LogLevel.Debug) {
			Log (LogCategory.Deck, message, level);
		}

		/// <summary>
		/// Log rule validation and game rules
		/// </summary>
		public static void LogRules (string message, LogLevel level = LogLevel.Debug) {
			Log (LogCategory.Rules, message, level);
		}

		/// <summary>
		/// Log system integration, events, and component connections
		/// </summary>
		public static void LogSystem (string message, LogLevel level = LogLevel.Debug) {
			Log (LogCategory.System, message, level);
		}

		/// <summary>
		/// Log diagnostic and debug information
		/// </summary>
		public static void LogDiagnostics (string message, LogLevel level = LogLevel.Trace) {
			Log (LogCategory.Diagnostics, message, level);
		}

		/// <summary>
		/// Log personal Investigation
		/// </summary>
		public static void LogInvestigation (string message, LogLevel level = LogLevel.Trace) {
			Log (LogCategory.Investigation, message, level);
		}

		/// <summary>
		/// PHASE 7: Log special card effects and validation
		/// </summary>
		public static void LogSpecialCards (string message, LogLevel level = LogLevel.Info) {
			Log (LogCategory.SpecialCards, message, level);
		}

		/// <summary>
		/// Log network-specific message
		/// </summary>
		public static void LogNetwork (string message, LogLevel level = LogLevel.Info) {
			Log (LogCategory.Network, message, level);
		}

		/// <summary>
		/// Log multiplayer-specific message  
		/// </summary>
		public static void LogMultiplayer (string message, LogLevel level = LogLevel.Info) {
			Log (LogCategory.Multiplayer, message, level);
		}

		// Direct level-based logging methods

		/// <summary>
		/// Log error messages (always shown unless level is Off)
		/// </summary>
		public static void LogError (string message, LogCategory category = LogCategory.System) {
			Log (category, message, LogLevel.Error);
		}

		/// <summary>
		/// Log warning messages
		/// </summary>
		public static void LogWarning (string message, LogCategory category = LogCategory.System) {
			Log (category, message, LogLevel.Warning);
		}

		/// <summary>
		/// Log info messages (important game events)
		/// </summary>
		public static void LogInfo (string message, LogCategory category = LogCategory.System) {
			Log (category, message, LogLevel.Info);
		}

		/// <summary>
		/// Log debug messages (detailed flow information)
		/// </summary>
		public static void LogDebug (string message, LogCategory category = LogCategory.System) {
			Log (category, message, LogLevel.Debug);
		}

		/// <summary>
		/// Log trace messages (verbose detailed information)
		/// </summary>
		public static void LogTrace (string message, LogCategory category = LogCategory.System) {
			Log (category, message, LogLevel.Trace);
		}

		// Core logging method
		private static void Log (LogCategory category, string message, LogLevel level) {
			if (!ShouldLog (category, level)) return;

			string formattedMessage = FormatMessage (category, message);

			// Route to appropriate Unity log level
			switch (level) {
				case LogLevel.Error:
					Debug.LogError (formattedMessage);
					break;
				case LogLevel.Warning:
					Debug.LogWarning (formattedMessage);
					break;
				default:
					Debug.Log (formattedMessage);
					break;
			}
		}

		// Helper methods

		/// <summary>
		/// Check if we should log at the specified level for the given category
		/// </summary>
		private static bool ShouldLog (LogCategory category, LogLevel level) {
			// If logging is completely off, don't log anything
			if (currentLogLevel == LogLevel.Off) {
				return false;
			}

			// In production mode, only show warnings and errors
			if (isProductionMode && level > LogLevel.Warning) {
				return false;
			}

			// Get category-specific log level, fallback to global level if not found
			LogLevel categoryLevel = categoryLogLevels.ContainsKey(category)
				? categoryLogLevels[category]
				: currentLogLevel;

			// Show this log if category level allows it (no global level check)
			return categoryLevel >= level;
		}

		/// <summary>
		/// Format message with category prefix
		/// </summary>
		private static string FormatMessage (LogCategory category, string message) {
			string categoryPrefix = GetCategoryPrefix (category);
			return $"[{categoryPrefix}] {message}";
		}

		/// <summary>
		/// Get short prefix for each category
		/// </summary>
		private static string GetCategoryPrefix (LogCategory category) {
			switch (category) {
				case LogCategory.TurnFlow: return "TURN";
				case LogCategory.CardPlay: return "CARD";
				case LogCategory.GameState: return "STATE";
				case LogCategory.TurnManagement: return "TURNS";
				case LogCategory.UI: return "UI";
				case LogCategory.AI: return "AI";
				case LogCategory.Deck: return "DECK";
				case LogCategory.Rules: return "RULES";
				case LogCategory.System: return "SYS";
				case LogCategory.Diagnostics: return "DIAG";
				case LogCategory.Investigation: return "INVESTIGATION";
				case LogCategory.SpecialCards: return "SPECIAL";
				case LogCategory.Network: return "NET";
				case LogCategory.Multiplayer: return "MP";
				default: return "GAME";
			}
		}

		// Configuration methods for GameManager

		/// <summary>
		/// Set log level (can be called at runtime)
		/// </summary>
		public static void SetLogLevel (LogLevel newLevel) {
			LogLevel oldLevel = currentLogLevel;
			currentLogLevel = newLevel;
			LogSystem ($"Log level changed from {oldLevel} to {newLevel}", LogLevel.Info);
		}

		/// <summary>
		/// Toggle production mode
		/// </summary>
		public static void SetProductionMode (bool productionMode) {
			isProductionMode = productionMode;
			string mode = productionMode ? "PRODUCTION" : "DEVELOPMENT";
			LogSystem ($"Switched to {mode} mode", LogLevel.Info);
		}

		/// <summary> 
		/// Get current configuration info
		/// </summary>
		public static string GetLoggerInfo () {
			string mode = isProductionMode ? "Production" : "Development";
			return $"TakiLogger - Level: {currentLogLevel}, Mode: {mode}";
		}

		/// <summary>
		/// Quick setup for common logging scenarios
		/// </summary>
		public static void SetupLogging(LogLevel level, bool production = false) {
			currentLogLevel = level;
			isProductionMode = production;
			LogInfo($"TakiLogger configured: Level={level}, Production={production}", LogCategory.System);
		}

		/// <summary>
		/// Enable/disable all logging
		/// </summary>
		public static void EnableLogging(bool enabled) {
			currentLogLevel = enabled ? LogLevel.Info : LogLevel.Off;
			LogInfo($"Logging {(enabled ? "enabled" : "disabled")}", LogCategory.System);
		}

		/// <summary>
		/// Set to development mode with verbose logging
		/// </summary>
		public static void EnableDebugMode() {
			SetupLogging(LogLevel.Debug, false);
		}

		/// <summary>
		/// Set to production mode with minimal logging
		/// </summary>
		public static void EnableProductionMode() {
			SetupLogging(LogLevel.Warning, true);
		}

		// Conditional compilation for release builds
		[System.Diagnostics.Conditional ("UNITY_EDITOR")]
		[System.Diagnostics.Conditional ("DEVELOPMENT_BUILD")]
		private static void EditorOnlyLog (string message) {
			// This will be removed in release builds
			Debug.Log (message);
		}
	}
}