import React from 'react';
import type { SkillCategoryMetric } from '@/api/analytics';
import { Award, Layers, Star, Code2 } from 'lucide-react';

interface SkillCoverageMatrixProps {
  categories: SkillCategoryMetric[];
}

export const SkillCoverageMatrix: React.FC<SkillCoverageMatrixProps> = ({ categories }) => {
  const totalSkillsCount = categories.reduce((sum, c) => sum + c.skillCount, 0);

  return (
    <div className="rounded-xl border border-border/60 bg-card p-5 shadow-sm space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between border-b border-border/40 pb-4 gap-2">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-lg bg-emerald-500/10 text-emerald-500">
            <Award className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-bold text-foreground">Skills Coverage & Mastery Catalog</h2>
            <p className="text-xs text-muted-foreground">
              Normalized master skills distribution, frequency, and proficiency ratings across resume revisions (ADR 006).
            </p>
          </div>
        </div>

        {totalSkillsCount > 0 && (
          <div className="flex items-center gap-2 text-xs">
            <span className="inline-flex items-center gap-1 font-semibold text-emerald-500 bg-emerald-500/10 px-2.5 py-1 rounded-full border border-emerald-500/20">
              <Layers className="h-3.5 w-3.5" /> {totalSkillsCount} Master Skills Cataloged
            </span>
          </div>
        )}
      </div>

      {categories.length === 0 ? (
        <div className="p-8 text-center text-sm text-muted-foreground">
          No normalized master skills attached to resume revisions yet.
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {categories.map((cat) => (
            <div
              key={cat.category}
              className="rounded-xl border border-border/60 bg-accent/20 p-4 space-y-3 hover:border-emerald-500/40 transition-all"
            >
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <div className="p-1.5 rounded-md bg-emerald-500/10 text-emerald-500">
                    <Layers className="h-4 w-4" />
                  </div>
                  <h3 className="font-bold text-sm text-foreground">{cat.categoryLabel}</h3>
                </div>

                <div className="flex items-center gap-2 text-xs">
                  <span className="font-semibold text-foreground bg-card px-2.5 py-0.5 rounded-full border border-border/40">
                    {cat.skillCount} skill(s)
                  </span>
                  <span className="font-bold text-emerald-500 flex items-center gap-1">
                    <Star className="h-3 w-3 fill-emerald-500" /> {cat.averageProficiency} / 5
                  </span>
                </div>
              </div>

              {/* Skill Tags Grid with Frequency & Proficiency */}
              <div className="flex flex-wrap gap-1.5 pt-1">
                {cat.skills.map((skill) => (
                  <div
                    key={skill.name}
                    className="inline-flex items-center gap-1.5 rounded-lg border border-border/50 bg-card px-2.5 py-1 text-xs font-medium text-foreground shadow-2xs hover:border-emerald-500/30 transition-colors"
                  >
                    <Code2 className="h-3 w-3 text-muted-foreground shrink-0" />
                    <span>{skill.name}</span>
                    <span className="text-[10px] font-bold text-emerald-500 bg-emerald-500/10 px-1.5 py-0.5 rounded">
                      Lvl {skill.proficiency}
                    </span>
                    {skill.count > 1 && (
                      <span
                        className="text-[10px] font-semibold text-muted-foreground bg-secondary px-1 py-0.5 rounded"
                        title={`Cataloged in ${skill.count} revisions`}
                      >
                        {skill.count} revs
                      </span>
                    )}
                    {skill.submittedAppCount > 0 && (
                      <span
                        className="text-[10px] font-semibold text-indigo-400 bg-indigo-500/10 px-1 py-0.5 rounded"
                        title={`Appears in ${skill.submittedAppCount} submitted application(s), ${skill.interviewAppCount} interview(s), ${skill.offerAppCount} offer(s)`}
                      >
                        {skill.submittedAppCount} apps
                      </span>
                    )}
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default SkillCoverageMatrix;
