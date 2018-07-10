﻿// Copyright (c) 2018 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows properties that override a property.
	/// </summary>
	[Export(typeof(IAnalyzer<IProperty>))]
	class PropertyOverriddenByAnalyzer : ITypeDefinitionAnalyzer<IProperty>
	{
		public string Text => "Overridden By";

		public IEnumerable<IEntity> Analyze(IProperty analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			if (!analyzedEntity.DeclaringType.GetAllBaseTypeDefinitions()
				.Any(t => t.MetadataToken == analyzedEntity.DeclaringTypeDefinition.MetadataToken && t.ParentAssembly.PEFile == type.ParentAssembly.PEFile))
				yield break;

			foreach (var property in type.Properties) {
				if (!property.IsOverride) continue;
				if (InheritanceHelper.GetBaseMembers(property, false)
					.Any(p => p.MetadataToken == analyzedEntity.MetadataToken &&
							  p.ParentAssembly.PEFile == analyzedEntity.ParentAssembly.PEFile)) {
					yield return property;
				}
			}
		}

		public bool Show(IProperty entity)
		{
			return entity.IsOverridable && entity.DeclaringType.Kind != TypeKind.Interface;
		}
	}
}