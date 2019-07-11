﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WorkspaceServer.Kernel;
using Xunit;

namespace WorkspaceServer.Tests.Kernel
{
    public class KernelCommandPipelineTests
    {
        [Fact(Skip = "WIP")]
        public void When_SubmitCode_command_adds_packages_to_fsharp_kernel_then_the_submission_is_passed_to_fsi()
        {
            throw new NotImplementedException();
        }

        [Fact(Skip = "WIP")]
        public void When_SubmitCode_command_adds_packages_to_fsharp_kernel_then_PackageAdded_event_is_raised()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task When_SubmitCode_command_adds_packages_to_csharp_kernel_then_the_submission_is_not_passed_to_csharpScript()
        {
            var kernel = new CompositeKernel(new[] { new CSharpRepl().UseNugetDirective() });

            var command = new SubmitCode("#r \"nuget:PocketLogger, 1.2.3\" \nvar a = new List<int>();", "csharp");
            await kernel.SendAsync(command);

            command.Code.Should().Be("var a = new List<int>();");
        }

        [Fact]
        public async Task When_SubmitCode_command_adds_packages_to_csharp_kernel_then_PackageAdded_event_is_raised()
        {
            var kernel = new CompositeKernel(new[] { new CSharpRepl().UseNugetDirective() });

            var command = new SubmitCode("#r \"nuget:PocketLogger, 1.2.3\" \nvar a = new List<int>();", "csharp");

            var result = await kernel.SendAsync(command);

            var events = result.KernelEvents
                               .ToEnumerable()
                               .ToArray();

            events
                .Should()
                .ContainSingle(e => e is NuGetPackageAdded);

            events.OfType<NuGetPackageAdded>()
                  .Single()
                  .Command
                  .As<AddNuGetPackage>()
                  .NugetReference
                  .Should()
                  .BeEquivalentTo(new NugetPackageReference("PocketLogger", "1.2.3"));
        }
    }
}