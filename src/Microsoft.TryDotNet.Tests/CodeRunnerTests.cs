﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.TryDotNet.WasmRunner;
using Xunit;

namespace Microsoft.TryDotNet.Tests
{
    public class CodeRunnerTests
    {
        [Fact]
        public async Task It_can_run_code_and_return_result()
        {
            var runner = new CodeRunner();
            var encodedAssembly = @"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mwAeAUABmwAeAWABmwAeAUIAaQBvAAMAAQAAAAYEmAAAAEMEnAACAAcAAwACAAkABQADAAoAGQADAAwAGwADAA4AHQADABAAHwADABIAIQADABQAIwADABYAJQAfAC4ABIAAAAEAAAAAAAAAAAAAAAAAzAAAAAQAAgABAAAAAAAAAHQA1AAAAAAABAABAAEAAAAAAAAAdABmAgAAAAAEAAIAAQAAAAAAAAB0ANECAAAAAAQAAQABAAAAAAAAAHQAvQAAAAAABAACAAEAAAAAAAAAdAChAwAAAAADAAIAKQBcAAAAADxjdXJyZW50PjVfXzEAPEZpYm9uYWNjaT5kX18xAElFbnVtZXJhYmxlYDEASUVudW1lcmF0b3JgMQBJbnQzMgA8bmV4dD41X18yADxNb2R1bGU+AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljAGdldF9DdXJyZW50TWFuYWdlZFRocmVhZElkADw+bF9faW5pdGlhbFRocmVhZElkAFRha2UASUVudW1lcmFibGUASURpc3Bvc2FibGUAU3lzdGVtLkNvbnNvbGUAY29uc29sZQBTeXN0ZW0uUnVudGltZQBXcml0ZUxpbmUAVHlwZQBTeXN0ZW0uSURpc3Bvc2FibGUuRGlzcG9zZQA8PjFfX3N0YXRlAENvbXBpbGVyR2VuZXJhdGVkQXR0cmlidXRlAERlYnVnZ2FibGVBdHRyaWJ1dGUAQXNzZW1ibHlUaXRsZUF0dHJpYnV0ZQBJdGVyYXRvclN0YXRlTWFjaGluZUF0dHJpYnV0ZQBEZWJ1Z2dlckhpZGRlbkF0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5SW5mb3JtYXRpb25hbFZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlDb25maWd1cmF0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQBTeXN0ZW0uRGlhZ25vc3RpY3MuRGVidWcARmlib25hY2NpAGNvbnNvbGUuZGxsAFByb2dyYW0AU3lzdGVtAE1haW4AU3lzdGVtLlJlZmxlY3Rpb24ATm90U3VwcG9ydGVkRXhjZXB0aW9uAFN5c3RlbS5MaW5xAElFbnVtZXJhdG9yAFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhYmxlPFN5c3RlbS5JbnQzMj4uR2V0RW51bWVyYXRvcgBTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmFibGUuR2V0RW51bWVyYXRvcgAuY3RvcgBTeXN0ZW0uRGlhZ25vc3RpY3MAU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcwBEZWJ1Z2dpbmdNb2RlcwBTeXN0ZW0uUnVudGltZS5FeHRlbnNpb25zAFN5c3RlbS5Db2xsZWN0aW9ucwBPYmplY3QAU3lzdGVtLkNvbGxlY3Rpb25zLklFbnVtZXJhdG9yLlJlc2V0AEVudmlyb25tZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhdG9yPFN5c3RlbS5JbnQzMj4uQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmF0b3IuQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5JRW51bWVyYXRvcjxTeXN0ZW0uSW50MzI+LmdldF9DdXJyZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5JRW51bWVyYXRvci5nZXRfQ3VycmVudAA8PjJfX2N1cnJlbnQATW92ZU5leHQAAAAAAPTh3kBkgaRGgtnPq+FiClEABCABAQgDIAABBSABARERBCABAQ4FIAEBEjUDIAACBRUSMQEIBCAAEwADIAAcBRUSPQEICCAAFRIxARMABCAAEkkIBwIVEjEBCAgQEAECFRI9AR4AFRI9AR4ACAMKAQgEAAEBCAMAAAgFBwMICAIEBwESDAiwP19/EdUKOgIGCAMAAAEHAAAVEj0BCAMgAAgHIAAVEjEBCAMoAAgDKAAcCAEACAAAAAAAHgEAAQBUAhZXcmFwTm9uRXhjZXB0aW9uVGhyb3dzAQgBAAcBAAAAAAwBAAdjb25zb2xlAAAKAQAFRGVidWcAAAwBAAcxLjAuMC4wAAAKAQAFMS4wLjAAABwBABdQcm9ncmFtKzxGaWJvbmFjY2k+ZF9fMQAABAEAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAFQsAAAAAAAAAAAAAG4sAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAABgLAAAAAAAAAAAAAAAAF9Db3JFeGVNYWluAG1zY29yZWUuZGxsAAAAAAD/JQAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAMAAAAgDwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            var outputs = new List<string>();
            var errors = new List<string>();
            var result = await runner.RunAssemblyEntryPoint(encodedAssembly, outputs.Add, errors.Add);

            result.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task It_produces_outputs()
        {
            var runner = new CodeRunner();
            var encodedAssembly = @"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mwAeAUABmwAeAWABmwAeAUIAaQBvAAMAAQAAAAYEmAAAAEMEnAACAAcAAwACAAkABQADAAoAGQADAAwAGwADAA4AHQADABAAHwADABIAIQADABQAIwADABYAJQAfAC4ABIAAAAEAAAAAAAAAAAAAAAAAzAAAAAQAAgABAAAAAAAAAHQA1AAAAAAABAABAAEAAAAAAAAAdABmAgAAAAAEAAIAAQAAAAAAAAB0ANECAAAAAAQAAQABAAAAAAAAAHQAvQAAAAAABAACAAEAAAAAAAAAdAChAwAAAAADAAIAKQBcAAAAADxjdXJyZW50PjVfXzEAPEZpYm9uYWNjaT5kX18xAElFbnVtZXJhYmxlYDEASUVudW1lcmF0b3JgMQBJbnQzMgA8bmV4dD41X18yADxNb2R1bGU+AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljAGdldF9DdXJyZW50TWFuYWdlZFRocmVhZElkADw+bF9faW5pdGlhbFRocmVhZElkAFRha2UASUVudW1lcmFibGUASURpc3Bvc2FibGUAU3lzdGVtLkNvbnNvbGUAY29uc29sZQBTeXN0ZW0uUnVudGltZQBXcml0ZUxpbmUAVHlwZQBTeXN0ZW0uSURpc3Bvc2FibGUuRGlzcG9zZQA8PjFfX3N0YXRlAENvbXBpbGVyR2VuZXJhdGVkQXR0cmlidXRlAERlYnVnZ2FibGVBdHRyaWJ1dGUAQXNzZW1ibHlUaXRsZUF0dHJpYnV0ZQBJdGVyYXRvclN0YXRlTWFjaGluZUF0dHJpYnV0ZQBEZWJ1Z2dlckhpZGRlbkF0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5SW5mb3JtYXRpb25hbFZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlDb25maWd1cmF0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQBTeXN0ZW0uRGlhZ25vc3RpY3MuRGVidWcARmlib25hY2NpAGNvbnNvbGUuZGxsAFByb2dyYW0AU3lzdGVtAE1haW4AU3lzdGVtLlJlZmxlY3Rpb24ATm90U3VwcG9ydGVkRXhjZXB0aW9uAFN5c3RlbS5MaW5xAElFbnVtZXJhdG9yAFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhYmxlPFN5c3RlbS5JbnQzMj4uR2V0RW51bWVyYXRvcgBTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmFibGUuR2V0RW51bWVyYXRvcgAuY3RvcgBTeXN0ZW0uRGlhZ25vc3RpY3MAU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcwBEZWJ1Z2dpbmdNb2RlcwBTeXN0ZW0uUnVudGltZS5FeHRlbnNpb25zAFN5c3RlbS5Db2xsZWN0aW9ucwBPYmplY3QAU3lzdGVtLkNvbGxlY3Rpb25zLklFbnVtZXJhdG9yLlJlc2V0AEVudmlyb25tZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5HZW5lcmljLklFbnVtZXJhdG9yPFN5c3RlbS5JbnQzMj4uQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuSUVudW1lcmF0b3IuQ3VycmVudABTeXN0ZW0uQ29sbGVjdGlvbnMuR2VuZXJpYy5JRW51bWVyYXRvcjxTeXN0ZW0uSW50MzI+LmdldF9DdXJyZW50AFN5c3RlbS5Db2xsZWN0aW9ucy5JRW51bWVyYXRvci5nZXRfQ3VycmVudAA8PjJfX2N1cnJlbnQATW92ZU5leHQAAAAAAPTh3kBkgaRGgtnPq+FiClEABCABAQgDIAABBSABARERBCABAQ4FIAEBEjUDIAACBRUSMQEIBCAAEwADIAAcBRUSPQEICCAAFRIxARMABCAAEkkIBwIVEjEBCAgQEAECFRI9AR4AFRI9AR4ACAMKAQgEAAEBCAMAAAgFBwMICAIEBwESDAiwP19/EdUKOgIGCAMAAAEHAAAVEj0BCAMgAAgHIAAVEjEBCAMoAAgDKAAcCAEACAAAAAAAHgEAAQBUAhZXcmFwTm9uRXhjZXB0aW9uVGhyb3dzAQgBAAcBAAAAAAwBAAdjb25zb2xlAAAKAQAFRGVidWcAAAwBAAcxLjAuMC4wAAAKAQAFMS4wLjAAABwBABdQcm9ncmFtKzxGaWJvbmFjY2k+ZF9fMQAABAEAAAAAAAAAAAAAAAAAAAAAEAAAAAAAAAAAAAAAAAAAAFQsAAAAAAAAAAAAAG4sAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAABgLAAAAAAAAAAAAAAAAF9Db3JFeGVNYWluAG1zY29yZWUuZGxsAAAAAAD/JQAgQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAMAAAAgDwAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
            var outputs = new List<string>();
            var errors = new List<string>();
            await runner.RunAssemblyEntryPoint(encodedAssembly, outputs.Add, errors.Add);

            outputs.Select(o => o.Trim()).Should().BeEquivalentTo(
                 "1", "1", "2", "3", "5", "8", "13", "21",
                 "34", "55", "89", "144", "233", "377", "610",
                 "987", "1597", "2584", "4181", "6765");

        }


        [Fact]
        public async Task It_can_run_a_private_main_method()
        {
            /* Compiled from 
using System;
namespace myApp
{ 
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
*/
            var runner = new CodeRunner();
            var encodedAssembly = @"TVqQAAMAAAAEAAAA//8AALgAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAgAAAAA4fug4AtAnNIbgBTM0hVGhpcyBwcm9ncmFtIGNhbm5vdCBiZSBydW4gaW4gRE9TIG1vZGUuDQ0KJAAAAAAAAABQRQAATAECAG+WAqcAAAAAAAAAAOAAIiALATAAAAYAAAACAAAAAAAAYiUAAAAgAAAAQAAAAAAAEAAgAAAAAgAABAAAAAAAAAAEAAAAAAAAAABgAAAAAgAAAAAAAAMAQIUAABAAABAAAAAAEAAAEAAAAAAAABAAAAAAAAAAAAAAABAlAABPAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEAAAAwAAAD0JAAAHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAACAAAAAAAAAAAAAAACCAAAEgAAAAAAAAAAAAAAC50ZXh0AAAAaAUAAAAgAAAABgAAAAIAAAAAAAAAAAAAAAAAACAAAGAucmVsb2MAAAwAAAAAQAAAAAIAAAAIAAAAAAAAAAAAAAAAAABAAABCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABEJQAAAAAAAEgAAAACAAUAcCAAAIQEAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADYAcgEAAHAoCgAACgAqIgIoCwAACgAqIgIoCwAACgAqQlNKQgEAAQAAAAAADAAAAHY0LjAuMzAzMTkAAAAABQBsAAAAhAEAACN+AADwAQAA2AEAACNTdHJpbmdzAAAAAMgDAAAcAAAAI1VTAOQDAAAQAAAAI0dVSUQAAAD0AwAAkAAAACNCbG9iAAAAAAAAAAIAAAFHFAAACQAAAAD6ATMAFgAAAQAAAAwAAAADAAAAAwAAAAsAAAAJAAAAAQAAAAEAAAAAAEoBAQAAAAAABgDaAKIBBgAsAaIBBgBNAI8BDwDCAQAABgATAXEBBgC7AHEBBgB4AHEBBgCVAHEBBgD6AHEBBgBhAHEBBgDRAWUBBgAdAGUBAAAAAAgAAAAAAAEAAQAAABAAXQGDAS0AAQABAAEAEAABADQALQABAAMAUCAAAAAAkQBsASMAAQBeIAAAAACGGIkBBgABAGcgAAAAAIYYiQEGAAEACQCJAQEAEQCJAQYAGQCJAQoAKQCJARAAMQCJARAAOQCJARAAQQCJARAASQCJARAAUQCJARAAYQBDABUAWQCJAQYALgALACcALgATADAALgAbAE8ALgAjAFgALgArAGwALgAzAHcALgA7AIQALgBDAFgALgBLAFgABIAAAAEAAAAAAAAAAAAAAAAAJQAAAAIAAAAAAAAAAAAAABoAEQAAAAAAAAAAAABDbGFzczEAPE1vZHVsZT4AbmV0c3RhbmRhcmQAQ29uc29sZQBibGF6b3ItY29uc29sZQBibGF6b3JfY29uc29sZQBXcml0ZUxpbmUARGVidWdnYWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5RmlsZVZlcnNpb25BdHRyaWJ1dGUAQXNzZW1ibHlJbmZvcm1hdGlvbmFsVmVyc2lvbkF0dHJpYnV0ZQBBc3NlbWJseUNvbmZpZ3VyYXRpb25BdHRyaWJ1dGUAQ29tcGlsYXRpb25SZWxheGF0aW9uc0F0dHJpYnV0ZQBBc3NlbWJseVByb2R1Y3RBdHRyaWJ1dGUAQXNzZW1ibHlDb21wYW55QXR0cmlidXRlAFJ1bnRpbWVDb21wYXRpYmlsaXR5QXR0cmlidXRlAGJsYXpvci1jb25zb2xlLmRsbABQcm9ncmFtAFN5c3RlbQBNYWluAFN5c3RlbS5SZWZsZWN0aW9uAG15QXBwAC5jdG9yAFN5c3RlbS5EaWFnbm9zdGljcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAERlYnVnZ2luZ01vZGVzAE9iamVjdAAAGUgAZQBsAGwAbwAgAFcAbwByAGwAZAAhAAAAXQ9GanmtTEuzgc0NDhYR8wAEIAEBCAMgAAEFIAEBEREEIAEBDgQAAQEOCMx7E//NLd1RAwAAAQgBAAgAAAAAAB4BAAEAVAIWV3JhcE5vbkV4Y2VwdGlvblRocm93cwEIAQAHAQAAAAATAQAOYmxhem9yLWNvbnNvbGUAAAoBAAVEZWJ1ZwAADAEABzEuMC4wLjAAAAoBAAUxLjAuMAAAAAAAAAAAAAAAAAAAABAAAAAAAAAAAAAAAAAAAAA4JQAAAAAAAAAAAABSJQAAACAAAAAAAAAAAAAAAAAAAAAAAAAAAAAARCUAAAAAAAAAAAAAAABfQ29yRGxsTWFpbgBtc2NvcmVlLmRsbAAAAAAA/yUAIAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAADAAAAGQ1AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
            var outputs = new List<string>();
            var errors = new List<string>();
            var result = await runner.RunAssemblyEntryPoint(encodedAssembly, outputs.Add, errors.Add);
            result.Succeeded.Should().BeTrue();
            outputs.Select(o => o.Trim()).Should().BeEquivalentTo("Hello World!");
        }

        [Fact]
        public async Task It_provides_errors_for_missing_type_at_runtime()
        {
            var runner = new CodeRunner();
            var outputs = new List<string>();
            var errors = new List<string>();
            var referencedCompilation = Compile(@"
public class C
{ 
}");

            var text = @"
class D
{
    public static void Main()
    {
        C c= new C();
    }
}";
            var other = Compile(text, referencedCompilation.ToMetadataReference());


            await using var stream = new MemoryStream();
            other.Emit(peStream: stream);
            var encodedAssembly = Convert.ToBase64String(stream.ToArray());


            var result = await runner.RunAssemblyEntryPoint(encodedAssembly, outputs.Add, errors.Add);
            result.Succeeded.Should().BeFalse();
            result.RunnerException.Message.Should().Match("*Could not load type 'C'*");
        }


        [Fact]
        public async Task It_returns_an_error_if_there_is_no_entry_point()
        {
            var runner = new CodeRunner();
            var outputs = new List<string>();
            var errors = new List<string>();
            var compilation = Compile("class C {}");

            await using var stream = new MemoryStream();
            compilation.Emit(peStream: stream);
            var encodedAssembly = Convert.ToBase64String(stream.ToArray());

            var result = await runner.RunAssemblyEntryPoint(encodedAssembly, outputs.Add, errors.Add);
            result.Succeeded.Should().BeFalse();
            result.RunnerException.Message.Should().Be(
                "Could not find a static entry point 'Main'.");
        }


        private static Compilation Compile(string text, params MetadataReference[] additionalReferences)
        {
            var refs = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(decimal).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Linq.Enumerable).Assembly.Location),
            }.Concat(additionalReferences);

            return CSharpCompilation.Create("assembly.dll", new[] { CSharpSyntaxTree.ParseText(text) }, refs,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }
    }
}
