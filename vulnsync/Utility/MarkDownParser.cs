using Markdig;
using Markdig.SyntaxHighlighting;

namespace VulnSync.Utility;

public class MarkDownParser
{
    private string _markdownContent;
    public MarkDownParser(string markdownContent)
    {
        this._markdownContent = markdownContent;
    }
    
    public string Parse()
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseSyntaxHighlighting().Build();
        var result = Markdig.Markdown.ToHtml(this._markdownContent, pipeline); 
        return result;
    }
}