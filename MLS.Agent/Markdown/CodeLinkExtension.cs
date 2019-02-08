﻿using Markdig;
using Markdig.Renderers;
using WorkspaceServer;

namespace MLS.Agent.Markdown
{
    public class CodeLinkExtension : IMarkdownExtension
    {
        private readonly IDirectoryAccessor _directoryAccessor;
        private readonly PackageRegistry _packageRegistry;

        public CodeLinkExtension(IDirectoryAccessor directoryAccessor, PackageRegistry packageRegistry)
        {
            _directoryAccessor = directoryAccessor;
            this._packageRegistry = packageRegistry;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Contains<CodeLinkBlockParser>())
            {
                // It should execute before the FencedCodeBlockParser
                pipeline.BlockParsers.Insert(0, new CodeLinkBlockParser(_directoryAccessor, _packageRegistry));
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            var renderers = htmlRenderer?.ObjectRenderers;
            if (renderers != null && !renderers.Contains<CodeLinkBlockRenderer>())
            {
                renderers.Insert(0, new CodeLinkBlockRenderer());
            }
        }
    }
}