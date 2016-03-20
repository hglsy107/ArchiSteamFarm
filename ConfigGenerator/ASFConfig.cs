﻿/*
    _                _      _  ____   _                           _____
   / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
  / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
 / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
/_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|

 Copyright 2015-2016 Łukasz "JustArchi" Domeradzki
 Contact: JustArchi@JustArchi.net

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0
					
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.

*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConfigGenerator {
	internal class ASFConfig {
		internal static List<ASFConfig> ASFConfigs = new List<ASFConfig>();

		internal string FilePath { get; set; }

		protected ASFConfig() {
			ASFConfigs.Add(this);
		}

		protected ASFConfig(string filePath) {
			FilePath = filePath;
			ASFConfigs.Add(this);
		}

		internal virtual void Save() {
			lock (FilePath) {
				try {
					File.WriteAllText(FilePath, JsonConvert.SerializeObject(this, Formatting.Indented));
				} catch (Exception e) {
					Logging.LogGenericException(e);
				}
			}
		}

		internal virtual void Remove() {
			string queryPath = Path.GetFileNameWithoutExtension(FilePath);
			lock (FilePath) {
				foreach (var configFile in Directory.EnumerateFiles(Program.ConfigDirectory, queryPath + ".*")) {
					try {
						File.Delete(configFile);
					} catch (Exception e) {
						Logging.LogGenericException(e);
					}
				}
			}
			ASFConfigs.Remove(this);
		}

		internal virtual void Rename(string botName) {
			if (string.IsNullOrEmpty(botName)) {
				return;
			}

			string queryPath = Path.GetFileNameWithoutExtension(FilePath);
			lock (FilePath) {
				foreach (var file in Directory.EnumerateFiles(Program.ConfigDirectory, queryPath + ".*")) {
					try {
						File.Move(file, Path.Combine(Program.ConfigDirectory, botName + Path.GetExtension(file)));
					} catch (Exception e) {
						Logging.LogGenericException(e);
					}
				}
				FilePath = Path.Combine(Program.ConfigDirectory, botName + ".json");
			}
		}
	}
}
