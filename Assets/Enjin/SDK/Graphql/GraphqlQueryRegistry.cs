/* Copyright 2021 Enjin Pte. Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using JetBrains.Annotations;

namespace Enjin.SDK.Graphql
{
    /// <summary>
    /// Class for registering and storing GraphQL templates.
    /// </summary>
    [PublicAPI]
    public class GraphqlQueryRegistry
    {
        private static readonly Regex TEMPLATE_REGEX =
            new Regex(new StringBuilder("^.+(?=schemas)") // Handles arbitrary number of path elements.
                .Append("schemas\\\\(?:project|player|shared)\\\\") // Validate schema.
                .Append("(?<type>fragment|mutation|query)\\\\") // Gets the template type.
                .Append("(?:[a-zA-Z]{1,}?).gql$") // Validates the query name.
                .ToString());

        private readonly Dictionary<string, GraphqlTemplate> _fragments = new Dictionary<string, GraphqlTemplate>();
        private readonly Dictionary<string, GraphqlTemplate> _operations = new Dictionary<string, GraphqlTemplate>();

        /// <summary>
        /// Sole constructor.
        /// </summary>
        public GraphqlQueryRegistry()
        {
            RegisterSdkTemplates();
        }

        private static string[]? LoadTemplateContents(Assembly assembly, string name)
        {
            var contents = new List<string>();

            FileStream fileStream = new FileStream(name, FileMode.Open, FileAccess.Read);
            if (fileStream == null)
                return null;

            using (StreamReader sr = new StreamReader(fileStream))
            {
                while (!sr.EndOfStream)
                {
                    contents.Add(sr.ReadLine());
                }
            }

            //int counter = 0;
            //foreach (string it_content in contents)
            //{

            //    Debug.Log(counter + ": " + it_content);
            //    counter += 1;
            //}
            //using var stream = assembly.GetManifestResourceStream(name);
            //if (stream == null)
            //    return null;

            //            using var reader = new StreamReader(stream);

            //while (!reader.EndOfStream)
            //{
            //    contents.Add(counter + " : " + reader.ReadLine());
            //}

            return contents.ToArray();
        }

        private void LoadAndCacheTemplateContents(string[]? contents, TemplateType templateType)
        {
            if (contents == null)
                return;
            
            var id = GraphqlTemplate.ReadNamespace(contents);
            if (id == null)
                return;

//            Debug.Log(templateType + " _ " + id);
            if (templateType == TemplateType.FRAGMENT)
                _fragments.Add(id, new GraphqlTemplate(id, templateType, contents, _fragments));
            else if (templateType == TemplateType.MUTATION || templateType == TemplateType.QUERY)
            {
                _operations.Add(id, new GraphqlTemplate(id, templateType, contents, _fragments));
            }
        }

        private void LoadTemplatesInAssembly(Assembly assembly)
        {
            //foreach(string it_file in Directory.GetFiles("C:/Users/JPLan/cryptGame/Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/project/query"))
            //{
            //    Debug.Log(it_file);
            //}
            //            Debug.Log(assembly.GetManifestResourceNames().Length.ToString());
            //            foreach (var name in assembly.GetManifestResourceNames())
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/project/query").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/project/mutation").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/shared/query").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/shared/mutation").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/shared/fragment").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/player/query").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }
            foreach (FileInfo name in new DirectoryInfo("Assets/Enjin/SDK/Graphql/Templates/enjin/schemas/player/mutation").GetFiles())
            {
                var match = TEMPLATE_REGEX.Match(name.FullName);
                if (!match.Success)
                    continue;

                var type = match.Groups["type"].Value;
                if (Enum.TryParse(type, true, out TemplateType templateType))
                    LoadAndCacheTemplateContents(LoadTemplateContents(assembly, name.FullName), templateType);
            }

            foreach (var operation in _operations.Values)
            {
                operation.Compile();
            }
        }

        /// <summary>
        /// Registers the templates in an assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void RegisterTemplatesInAssembly(Assembly assembly)
        {
            LoadTemplatesInAssembly(assembly);
        }

        internal void RegisterSdkTemplates()
        {
            RegisterTemplatesInAssembly(typeof(GraphqlQueryRegistry).Assembly);
        }

        /// <summary>
        /// Gets the template that is registered under the name provided.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <returns>The template if one exists, else <c>null</c>.</returns>
        public GraphqlTemplate? GetOperationForName(string name)
        {
            return _operations.ContainsKey(name)
                ? _operations[name]
                : null;
        }
    }
}