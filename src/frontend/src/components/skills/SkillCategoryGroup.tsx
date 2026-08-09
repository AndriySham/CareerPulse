import React, { useState } from 'react';
import type { MasterSkillDto, SkillCategory } from '@/types';
import SkillCard, { CATEGORY_CONFIG } from './SkillCard';
import { ChevronDown } from 'lucide-react';

interface SkillCategoryGroupProps {
  category: SkillCategory;
  skills: MasterSkillDto[];
  defaultOpen?: boolean;
}

export const SkillCategoryGroup: React.FC<SkillCategoryGroupProps> = ({
  category,
  skills,
  defaultOpen = true,
}) => {
  const [isOpen, setIsOpen] = useState(defaultOpen);
  const categoryMeta = CATEGORY_CONFIG[category] || CATEGORY_CONFIG.Other;
  const CategoryIcon = categoryMeta.icon;

  if (skills.length === 0) return null;

  return (
    <div className="rounded-xl border border-border/60 bg-card/60 overflow-hidden transition-all">
      {/* Category Group Header */}
      <button
        type="button"
        onClick={() => setIsOpen(!isOpen)}
        className="w-full flex items-center justify-between p-4 bg-accent/30 hover:bg-accent/50 transition-colors text-left cursor-pointer border-b border-border/40"
      >
        <div className="flex items-center gap-3">
          <div className={`p-2 rounded-lg border ${categoryMeta.colorClass}`}>
            <CategoryIcon className="h-5 w-5" />
          </div>
          <div>
            <div className="flex items-center gap-2">
              <h2 className="text-base font-bold tracking-tight text-foreground">
                {categoryMeta.label}
              </h2>
              <span className="rounded-full bg-primary/10 px-2.5 py-0.5 text-xs font-semibold text-primary border border-primary/20">
                {skills.length} {skills.length === 1 ? 'skill' : 'skills'}
              </span>
            </div>
            <p className="text-xs text-muted-foreground mt-0.5">
              Canonical master skills categorized under {categoryMeta.label}
            </p>
          </div>
        </div>

        <ChevronDown
          className={`h-5 w-5 text-muted-foreground transition-transform duration-200 shrink-0 ${
            isOpen ? 'rotate-180' : ''
          }`}
        />
      </button>

      {/* Group Content Grid */}
      {isOpen && (
        <div className="p-4 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {skills.map((skill) => (
            <SkillCard key={skill.id} skill={skill} />
          ))}
        </div>
      )}
    </div>
  );
};

export default SkillCategoryGroup;
