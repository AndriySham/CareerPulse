import React from 'react';
import type { MasterSkillDto, SkillCategory } from '@/types';
import {
  Code2,
  Layers,
  GitMerge,
  Database,
  Cloud,
  Cpu,
  MessageSquare,
  CheckCircle2,
  Wrench,
  Users,
  Tag,
  ShieldCheck,
  Calendar,
} from 'lucide-react';

interface SkillCardProps {
  skill: MasterSkillDto;
}

export const CATEGORY_CONFIG: Record<
  SkillCategory,
  { label: string; icon: React.FC<{ className?: string }>; colorClass: string }
> = {
  ProgrammingLanguage: {
    label: 'Programming Language',
    icon: Code2,
    colorClass: 'bg-violet-500/10 text-violet-500 border-violet-500/20',
  },
  Framework: {
    label: 'Framework',
    icon: Layers,
    colorClass: 'bg-blue-500/10 text-blue-500 border-blue-500/20',
  },
  ORM: {
    label: 'ORM & Data Access',
    icon: GitMerge,
    colorClass: 'bg-indigo-500/10 text-indigo-500 border-indigo-500/20',
  },
  Database: {
    label: 'Database',
    icon: Database,
    colorClass: 'bg-emerald-500/10 text-emerald-500 border-emerald-500/20',
  },
  Cloud: {
    label: 'Cloud Infrastructure',
    icon: Cloud,
    colorClass: 'bg-sky-500/10 text-sky-500 border-sky-500/20',
  },
  DevOps: {
    label: 'DevOps & CI/CD',
    icon: Cpu,
    colorClass: 'bg-amber-500/10 text-amber-500 border-amber-500/20',
  },
  Messaging: {
    label: 'Messaging & Queues',
    icon: MessageSquare,
    colorClass: 'bg-rose-500/10 text-rose-500 border-rose-500/20',
  },
  Testing: {
    label: 'Testing & QA',
    icon: CheckCircle2,
    colorClass: 'bg-teal-500/10 text-teal-500 border-teal-500/20',
  },
  Tools: {
    label: 'Tools & Utilities',
    icon: Wrench,
    colorClass: 'bg-orange-500/10 text-orange-500 border-orange-500/20',
  },
  SoftSkill: {
    label: 'Soft Skills',
    icon: Users,
    colorClass: 'bg-pink-500/10 text-pink-500 border-pink-500/20',
  },
  Other: {
    label: 'Other',
    icon: Tag,
    colorClass: 'bg-slate-500/10 text-slate-500 border-slate-500/20',
  },
};

export const SkillCard: React.FC<SkillCardProps> = ({ skill }) => {
  const categoryMeta = CATEGORY_CONFIG[skill.category] || CATEGORY_CONFIG.Other;
  const CategoryIcon = categoryMeta.icon;

  return (
    <div className="group relative flex flex-col justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm transition-all hover:border-primary/40 hover:shadow-md">
      <div>
        {/* Top row: Category Badge & Status */}
        <div className="flex items-center justify-between gap-2 mb-3">
          <span
            className={`inline-flex items-center gap-1.5 rounded-lg border px-2.5 py-1 text-xs font-semibold ${categoryMeta.colorClass}`}
          >
            <CategoryIcon className="h-3.5 w-3.5" />
            {categoryMeta.label}
          </span>

          {!skill.isActive && (
            <span className="rounded-md bg-muted px-2 py-0.5 text-[11px] font-semibold text-muted-foreground border border-border">
              Inactive
            </span>
          )}
        </div>

        {/* Canonical Skill Name */}
        <div className="flex items-center gap-2">
          <h3 className="font-bold text-base text-foreground tracking-tight group-hover:text-primary transition-colors">
            {skill.name}
          </h3>
          <span
            className="inline-flex items-center gap-0.5 text-[10px] font-semibold uppercase tracking-wider text-emerald-500 bg-emerald-500/10 px-1.5 py-0.5 rounded border border-emerald-500/20"
            title="Canonical MasterSkill in Single Source of Truth catalog (ADR 006)"
          >
            <ShieldCheck className="h-3 w-3" /> Master
          </span>
        </div>

        {/* Aliases Section */}
        {skill.aliases && skill.aliases.length > 0 ? (
          <div className="mt-3 space-y-1">
            <span className="text-[11px] font-medium text-muted-foreground block">
              Aliases ({skill.aliases.length}):
            </span>
            <div className="flex flex-wrap gap-1">
              {skill.aliases.map((alias) => (
                <span
                  key={alias}
                  className="inline-flex items-center rounded-md bg-accent/60 px-2 py-0.5 text-xs text-foreground/80 border border-border/30"
                >
                  {alias}
                </span>
              ))}
            </div>
          </div>
        ) : (
          <p className="mt-3 text-xs text-muted-foreground/50 italic">No aliases defined</p>
        )}
      </div>

      {/* Footer Meta */}
      <div className="mt-4 pt-3 border-t border-border/40 flex items-center justify-between text-xs text-muted-foreground">
        <span className="flex items-center gap-1 text-[11px]">
          <Calendar className="h-3 w-3 text-primary/70 shrink-0" />
          Added {new Date(skill.createdAt).toLocaleDateString()}
        </span>
      </div>
    </div>
  );
};

export default SkillCard;
