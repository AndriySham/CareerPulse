import React from 'react';
import { FileText, Sparkles } from 'lucide-react';

interface ProfessionalSummarySectionProps {
  summary: string;
  onChange: (summary: string) => void;
  isReadOnly?: boolean;
}

export const ProfessionalSummarySection: React.FC<ProfessionalSummarySectionProps> = ({
  summary,
  onChange,
  isReadOnly = false,
}) => {
  const charCount = summary.trim().length;

  const getLengthBadge = () => {
    if (charCount === 0) return null;
    if (charCount < 100) {
      return (
        <span className="text-[11px] font-medium text-amber-400 bg-amber-500/10 px-2 py-0.5 rounded-full border border-amber-500/20">
          Too short (Aim for 200+ chars)
        </span>
      );
    }
    if (charCount <= 800) {
      return (
        <span className="text-[11px] font-medium text-emerald-400 bg-emerald-500/10 px-2 py-0.5 rounded-full border border-emerald-500/20">
          Optimal Length
        </span>
      );
    }
    return (
      <span className="text-[11px] font-medium text-blue-400 bg-blue-500/10 px-2 py-0.5 rounded-full border border-blue-500/20">
        Detailed Summary
      </span>
    );
  };

  return (
    <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-4">
      {/* Section Header */}
      <div className="flex items-center justify-between border-b border-border/40 pb-4">
        <div className="flex items-center gap-3">
          <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <FileText className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-semibold text-foreground">Professional Summary</h2>
            <p className="text-xs text-muted-foreground">
              Overview of software engineering expertise, technical focus, and accomplishments.
            </p>
          </div>
        </div>

        {/* Character Count & Quality Indicator */}
        <div className="flex items-center gap-2">
          {getLengthBadge()}
          <span className="text-xs font-mono text-muted-foreground bg-accent/40 px-2.5 py-1 rounded-lg border border-border/40">
            {charCount} chars
          </span>
        </div>
      </div>

      {/* Textarea */}
      <div className="space-y-2">
        <label className="block text-xs font-medium text-muted-foreground flex items-center justify-between">
          <span>
            Executive Summary Text <span className="text-destructive">*</span>
          </span>
        </label>
        <textarea
          required
          rows={5}
          disabled={isReadOnly}
          value={summary}
          onChange={(e) => onChange(e.target.value)}
          placeholder="Write a compelling overview of your software engineering expertise, primary tech stack (.NET, React, PostgreSQL), architectural achievements, and career goals..."
          className="w-full rounded-lg border border-border bg-background p-3 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-60 transition-colors leading-relaxed"
        />
      </div>

      {/* Developer Tip Card */}
      {!isReadOnly && (
        <div className="rounded-xl border border-primary/20 bg-primary/5 p-3.5 flex items-start gap-3 text-xs text-muted-foreground">
          <Sparkles className="h-4 w-4 text-primary shrink-0 mt-0.5" />
          <div>
            <span className="font-semibold text-foreground">Pro-tip for Software Engineers:</span>{' '}
            Mention your primary specialization (e.g. C# .NET Backend, Full-Stack React), years of experience, core technical achievements (e.g. API design, DB optimization, CQRS architecture), and key domain experience.
          </div>
        </div>
      )}
    </div>
  );
};

export default ProfessionalSummarySection;
