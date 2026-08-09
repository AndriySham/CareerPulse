import React, { useState, useMemo } from 'react';
import { useMasterSkills } from '@/api/masterSkills';
import SkillCard, { CATEGORY_CONFIG } from '@/components/skills/SkillCard';
import SkillCategoryGroup from '@/components/skills/SkillCategoryGroup';
import SkillFormModal from '@/components/skills/SkillFormModal';
import CustomSelect from '@/components/ui/CustomSelect';
import type { SkillCategory } from '@/types';
import {
  Award,
  Search,
  Plus,
  Filter,
  CheckCircle2,
  GitMerge,
  Layers,
  LayoutGrid,
  ListFilter,
  SlidersHorizontal,
} from 'lucide-react';

const CATEGORY_FILTER_OPTIONS = [
  { value: 'all', label: 'All Categories' },
  ...(Object.keys(CATEGORY_CONFIG) as SkillCategory[]).map((cat) => ({
    value: cat,
    label: CATEGORY_CONFIG[cat].label,
  })),
];

export const SkillsPage: React.FC = () => {
  const [selectedCategory, setSelectedCategory] = useState<string>('all');
  const [includeInactive, setIncludeInactive] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [viewMode, setViewMode] = useState<'grouped' | 'grid'>('grouped');

  // Modal State
  const [isSkillFormOpen, setIsSkillFormOpen] = useState(false);

  // API Query
  const categoryParam = selectedCategory === 'all' ? undefined : (selectedCategory as SkillCategory);
  const { data: skills = [], isLoading, isError, refetch } = useMasterSkills(categoryParam, includeInactive);

  // Statistics
  const totalSkills = skills.length;
  const activeSkillsCount = useMemo(() => skills.filter((s) => s.isActive).length, [skills]);
  const totalAliasesCount = useMemo(
    () => skills.reduce((acc, s) => acc + (s.aliases?.length || 0), 0),
    [skills]
  );
  const categoriesCount = useMemo(() => {
    const set = new Set(skills.map((s) => s.category));
    return set.size;
  }, [skills]);

  // Search Filter
  const filteredSkills = useMemo(() => {
    if (!searchQuery.trim()) return skills;
    const query = searchQuery.toLowerCase();
    return skills.filter(
      (s) =>
        s.name.toLowerCase().includes(query) ||
        s.aliases?.some((a) => a.toLowerCase().includes(query))
    );
  }, [skills, searchQuery]);

  // Grouped skills by category
  const skillsByCategory = useMemo(() => {
    const map = new Map<SkillCategory, typeof skills>();
    (Object.keys(CATEGORY_CONFIG) as SkillCategory[]).forEach((cat) => {
      map.set(cat, []);
    });

    filteredSkills.forEach((skill) => {
      const list = map.get(skill.category) || [];
      list.push(skill);
      map.set(skill.category, list);
    });

    return map;
  }, [filteredSkills]);

  const handleCreateSkill = () => {
    setIsSkillFormOpen(true);
  };

  return (
    <div className="space-y-6">
      {/* Header section */}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between border-b border-border/40 pb-5">
        <div className="flex items-center gap-3">
          <div className="p-2.5 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <Award className="h-6 w-6" />
          </div>
          <div>
            <h1 className="text-2xl font-bold tracking-tight text-foreground">
              Master Skills Catalog
            </h1>
            <p className="text-sm text-muted-foreground">
              Manage canonical technical skills, normalized categories, and alias mappings (ADR 006).
            </p>
          </div>
        </div>

        <button
          type="button"
          onClick={handleCreateSkill}
          className="inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2.5 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
        >
          <Plus className="h-4 w-4" /> Add Master Skill
        </button>
      </div>

      {/* Summary KPI Cards */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-primary/10 text-primary">
            <Award className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{totalSkills}</div>
            <div className="text-xs text-muted-foreground font-medium">Master Skills</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-emerald-500/10 text-emerald-500">
            <CheckCircle2 className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{activeSkillsCount}</div>
            <div className="text-xs text-muted-foreground font-medium">Active Skills</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-indigo-500/10 text-indigo-500">
            <GitMerge className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{totalAliasesCount}</div>
            <div className="text-xs text-muted-foreground font-medium">Mapped Aliases</div>
          </div>
        </div>

        <div className="rounded-xl border border-border/60 bg-card p-4 shadow-sm flex items-center gap-4">
          <div className="p-3 rounded-lg bg-amber-500/10 text-amber-500">
            <Layers className="h-5 w-5" />
          </div>
          <div>
            <div className="text-2xl font-bold text-foreground">{categoriesCount}</div>
            <div className="text-xs text-muted-foreground font-medium">Categories In Use</div>
          </div>
        </div>
      </div>

      {/* Filter and Search Bar */}
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between rounded-xl border border-border/60 bg-card p-4 shadow-sm">
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search skills by canonical name or alias..."
            className="w-full rounded-lg border border-border bg-background pl-9 pr-4 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div className="flex flex-wrap items-center gap-3 text-sm">
          <div className="flex items-center gap-2">
            <Filter className="h-4 w-4 text-muted-foreground shrink-0" />
            <CustomSelect
              value={selectedCategory}
              onChange={(val) => setSelectedCategory(val)}
              options={CATEGORY_FILTER_OPTIONS}
              className="w-52"
            />
          </div>

          <label className="flex items-center gap-2 cursor-pointer text-xs font-semibold text-muted-foreground hover:text-foreground">
            <input
              type="checkbox"
              checked={includeInactive}
              onChange={(e) => setIncludeInactive(e.target.checked)}
              className="h-4 w-4 rounded border-border text-primary focus:ring-primary accent-primary"
            />
            <span className="flex items-center gap-1">
              <SlidersHorizontal className="h-3.5 w-3.5" /> Include Inactive
            </span>
          </label>

          <div className="flex items-center border border-border rounded-lg p-1 bg-background">
            <button
              type="button"
              onClick={() => setViewMode('grouped')}
              className={`p-1.5 rounded-md text-xs font-medium transition-colors ${
                viewMode === 'grouped'
                  ? 'bg-primary text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
              title="Group by Category"
            >
              <ListFilter className="h-4 w-4" />
            </button>
            <button
              type="button"
              onClick={() => setViewMode('grid')}
              className={`p-1.5 rounded-md text-xs font-medium transition-colors ${
                viewMode === 'grid'
                  ? 'bg-primary text-primary-foreground'
                  : 'text-muted-foreground hover:text-foreground'
              }`}
              title="Compact Grid View"
            >
              <LayoutGrid className="h-4 w-4" />
            </button>
          </div>
        </div>
      </div>

      {/* Content Section */}
      {isLoading ? (
        <div className="space-y-4">
          {[1, 2, 3].map((i) => (
            <div key={i} className="h-36 rounded-xl border border-border/60 bg-card p-5 animate-pulse space-y-3">
              <div className="h-5 w-1/3 bg-accent/60 rounded" />
              <div className="h-4 w-1/4 bg-accent/40 rounded" />
              <div className="h-12 w-full bg-accent/30 rounded mt-4" />
            </div>
          ))}
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-destructive/30 bg-destructive/10 p-8 text-center">
          <p className="text-sm text-destructive font-medium mb-2">Failed to load Master Skills catalog.</p>
          <button
            type="button"
            onClick={() => refetch()}
            className="rounded-lg bg-destructive px-4 py-2 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90 cursor-pointer"
          >
            Retry
          </button>
        </div>
      ) : filteredSkills.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-card p-12 text-center shadow-sm">
          <Award className="mx-auto h-12 w-12 text-muted-foreground/40 mb-3" />
          <h3 className="text-base font-semibold text-foreground">No master skills found</h3>
          <p className="mt-1 text-sm text-muted-foreground max-w-sm mx-auto">
            {searchQuery || selectedCategory !== 'all'
              ? 'No master skills matched your search query or category filter.'
              : 'Start populating your canonical skill catalog by adding your first master skill.'}
          </p>
          <button
            type="button"
            onClick={handleCreateSkill}
            className="mt-4 inline-flex items-center gap-2 rounded-xl bg-primary px-4 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
          >
            <Plus className="h-4 w-4" /> Add First Master Skill
          </button>
        </div>
      ) : viewMode === 'grouped' ? (
        <div className="space-y-4">
          {(Object.keys(CATEGORY_CONFIG) as SkillCategory[]).map((cat) => {
            const categorySkills = skillsByCategory.get(cat) || [];
            if (categorySkills.length === 0) return null;
            return <SkillCategoryGroup key={cat} category={cat} skills={categorySkills} />;
          })}
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {filteredSkills.map((skill) => (
            <SkillCard key={skill.id} skill={skill} />
          ))}
        </div>
      )}

      {/* Modal */}
      <SkillFormModal
        isOpen={isSkillFormOpen}
        onClose={() => setIsSkillFormOpen(false)}
        initialCategory={selectedCategory !== 'all' ? (selectedCategory as SkillCategory) : 'ProgrammingLanguage'}
      />
    </div>
  );
};

export default SkillsPage;
