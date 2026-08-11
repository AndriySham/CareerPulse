import React, { useState, useMemo } from 'react';
import type { MasterSkillDto, ResumeSkillInputDto, SkillCategory } from '@/types';
import { CATEGORY_CONFIG } from '@/components/skills/SkillCard';
import SkillFormModal from '@/components/skills/SkillFormModal';
import CustomSelect from '@/components/ui/CustomSelect';
import {
  Award,
  Plus,
  Trash2,
  Filter,
  Search,
  SlidersHorizontal,
} from 'lucide-react';

interface SkillsSectionProps {
  attachedSkills: ResumeSkillInputDto[];
  onChangeAttachedSkills: (skills: ResumeSkillInputDto[]) => void;
  masterSkills: MasterSkillDto[];
  isReadOnly?: boolean;
}

const PROFICIENCY_LABELS: Record<number, { title: string; short: string; color: string }> = {
  1: { title: '1 - Beginner', short: 'Beginner', color: 'text-amber-400 bg-amber-500/10 border-amber-500/20' },
  2: { title: '2 - Elementary', short: 'Elementary', color: 'text-yellow-400 bg-yellow-500/10 border-yellow-500/20' },
  3: { title: '3 - Intermediate', short: 'Intermediate', color: 'text-blue-400 bg-blue-500/10 border-blue-500/20' },
  4: { title: '4 - Advanced', short: 'Advanced', color: 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20' },
  5: { title: '5 - Expert', short: 'Expert', color: 'text-violet-400 bg-violet-500/10 border-violet-500/20' },
};

export const SkillsSection: React.FC<SkillsSectionProps> = ({
  attachedSkills,
  onChangeAttachedSkills,
  masterSkills,
  isReadOnly = false,
}) => {
  // Skill Selector Controls State
  const [selectedCategoryFilter, setSelectedCategoryFilter] = useState<string>('all');
  const [searchCatalogQuery, setSearchCatalogQuery] = useState('');
  const [selectedSkillId, setSelectedSkillId] = useState('');
  const [selectedProficiency, setSelectedProficiency] = useState(3);

  // Attached Skills Filter State
  const [attachedCategoryFilter, setAttachedCategoryFilter] = useState<string>('all');
  const [attachedSearchQuery, setAttachedSearchQuery] = useState('');

  // Skill Form Modal State
  const [isSkillModalOpen, setIsSkillModalOpen] = useState(false);

  // Filter MasterSkills catalog options for selection
  const filteredCatalogSkills = useMemo(() => {
    let result = masterSkills.filter(
      (ms) => !attachedSkills.some((as) => as.masterSkillId === ms.id)
    );

    if (selectedCategoryFilter !== 'all') {
      result = result.filter((ms) => ms.category === selectedCategoryFilter);
    }

    if (searchCatalogQuery.trim()) {
      const q = searchCatalogQuery.toLowerCase();
      result = result.filter(
        (ms) =>
          ms.name.toLowerCase().includes(q) ||
          ms.aliases?.some((alias) => alias.toLowerCase().includes(q))
      );
    }

    return result;
  }, [masterSkills, attachedSkills, selectedCategoryFilter, searchCatalogQuery]);

  const catalogSelectOptions = useMemo(() => {
    return filteredCatalogSkills.map((ms) => ({
      value: ms.id,
      label: `${ms.name} (${CATEGORY_CONFIG[ms.category]?.label || ms.category})`,
    }));
  }, [filteredCatalogSkills]);

  // Handlers
  const handleAddSkill = () => {
    if (!selectedSkillId) return;
    if (attachedSkills.some((s) => s.masterSkillId === selectedSkillId)) return;

    onChangeAttachedSkills([
      ...attachedSkills,
      { masterSkillId: selectedSkillId, proficiencyLevel: selectedProficiency },
    ]);
    setSelectedSkillId('');
    setSelectedProficiency(3);
  };

  const handleRemoveSkill = (skillId: string) => {
    onChangeAttachedSkills(attachedSkills.filter((s) => s.masterSkillId !== skillId));
  };

  const handleUpdateProficiency = (skillId: string, level: number) => {
    if (isReadOnly) return;
    onChangeAttachedSkills(
      attachedSkills.map((s) =>
        s.masterSkillId === skillId ? { ...s, proficiencyLevel: level } : s
      )
    );
  };

  // Filter Attached Skills for display
  const displayedAttachedSkills = useMemo(() => {
    let list = attachedSkills;

    if (attachedCategoryFilter !== 'all') {
      list = list.filter((s) => {
        const ms = masterSkills.find((m) => m.id === s.masterSkillId);
        return ms?.category === attachedCategoryFilter;
      });
    }

    if (attachedSearchQuery.trim()) {
      const q = attachedSearchQuery.toLowerCase();
      list = list.filter((s) => {
        const ms = masterSkills.find((m) => m.id === s.masterSkillId);
        return ms?.name.toLowerCase().includes(q);
      });
    }

    return list;
  }, [attachedSkills, masterSkills, attachedCategoryFilter, attachedSearchQuery]);

  // Category counts for attached skills
  const attachedCategoryCounts = useMemo(() => {
    const counts: Record<string, number> = { all: attachedSkills.length };
    attachedSkills.forEach((s) => {
      const ms = masterSkills.find((m) => m.id === s.masterSkillId);
      const cat = ms?.category || 'Other';
      counts[cat] = (counts[cat] || 0) + 1;
    });
    return counts;
  }, [attachedSkills, masterSkills]);

  return (
    <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 border-b border-border/40 pb-4">
        <div className="flex items-center gap-3">
          <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <Award className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-semibold text-foreground">Skills & Proficiency</h2>
            <p className="text-xs text-muted-foreground">
              Normalized technical skill mapping and proficiency levels (ADR 006).
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <span className="rounded-full bg-primary/10 border border-primary/20 px-3 py-1 text-xs font-semibold text-primary">
            {attachedSkills.length} attached skill(s)
          </span>
        </div>
      </div>

      {/* ADD SKILL CONTROLS CARD */}
      {!isReadOnly && (
        <div className="rounded-xl border border-border/60 bg-accent/20 p-4 space-y-4">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
            <div className="text-xs font-semibold text-foreground flex items-center gap-2">
              <SlidersHorizontal className="h-3.5 w-3.5 text-primary" />
              Attach Skill from Master Catalog
            </div>

            <button
              type="button"
              onClick={() => setIsSkillModalOpen(true)}
              className="inline-flex items-center gap-1.5 text-xs font-semibold text-primary hover:text-primary/80 transition-colors cursor-pointer self-start sm:self-auto"
            >
              <Plus className="h-3.5 w-3.5" />
              <span>Add New Master Skill to Catalog</span>
            </button>
          </div>

          {/* Filters & Dropdown Row */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            {/* Category Filter */}
            <div>
              <label className="block text-[11px] font-medium text-muted-foreground mb-1 flex items-center gap-1">
                <Filter className="h-3 w-3" /> Category Filter
              </label>
              <select
                value={selectedCategoryFilter}
                onChange={(e) => setSelectedCategoryFilter(e.target.value)}
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs font-medium text-foreground focus:outline-none focus:ring-2 focus:ring-primary cursor-pointer"
              >
                <option value="all">All Categories ({filteredCatalogSkills.length})</option>
                {(Object.keys(CATEGORY_CONFIG) as SkillCategory[]).map((cat) => (
                  <option key={cat} value={cat}>
                    {CATEGORY_CONFIG[cat].label}
                  </option>
                ))}
              </select>
            </div>

            {/* Catalog Search Input */}
            <div>
              <label className="block text-[11px] font-medium text-muted-foreground mb-1 flex items-center gap-1">
                <Search className="h-3 w-3" /> Search Catalog
              </label>
              <input
                type="text"
                value={searchCatalogQuery}
                onChange={(e) => setSearchCatalogQuery(e.target.value)}
                placeholder="Search by skill name or alias..."
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-xs placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            {/* Skill Selector */}
            <div>
              <label className="block text-[11px] font-medium text-muted-foreground mb-1">
                Select Skill
              </label>
              <CustomSelect
                value={selectedSkillId}
                onChange={(val) => setSelectedSkillId(val)}
                options={catalogSelectOptions}
                placeholder={
                  filteredCatalogSkills.length === 0
                    ? 'No matching catalog skills'
                    : 'Choose skill...'
                }
              />
            </div>
          </div>

          {/* Proficiency & Attach Button Row */}
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 pt-2 border-t border-border/40">
            <div className="flex items-center gap-3">
              <span className="text-xs font-medium text-muted-foreground whitespace-nowrap">
                Proficiency Level:
              </span>
              <div className="flex items-center gap-1">
                {[1, 2, 3, 4, 5].map((lvl) => (
                  <button
                    key={lvl}
                    type="button"
                    onClick={() => setSelectedProficiency(lvl)}
                    className={`px-2.5 py-1 rounded-lg text-xs font-semibold border transition-all cursor-pointer ${
                      selectedProficiency === lvl
                        ? PROFICIENCY_LABELS[lvl].color
                        : 'bg-background border-border text-muted-foreground hover:bg-accent'
                    }`}
                  >
                    {lvl}
                  </button>
                ))}
              </div>
              <span className="text-xs font-medium text-muted-foreground hidden md:inline">
                ({PROFICIENCY_LABELS[selectedProficiency].short})
              </span>
            </div>

            <button
              type="button"
              onClick={handleAddSkill}
              disabled={!selectedSkillId}
              className="inline-flex items-center justify-center gap-1.5 rounded-xl bg-primary px-5 py-2 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Plus className="h-3.5 w-3.5" /> Attach Skill to Revision
            </button>
          </div>
        </div>
      )}

      {/* ATTACHED SKILLS DISPLAY SECTION */}
      <div className="space-y-4">
        {/* Attached Skills Toolbar / Filter Pills */}
        {attachedSkills.length > 0 && (
          <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3">
            <div className="flex items-center gap-1.5 overflow-x-auto pb-1">
              <button
                type="button"
                onClick={() => setAttachedCategoryFilter('all')}
                className={`px-3 py-1 rounded-lg text-xs font-semibold transition-all cursor-pointer whitespace-nowrap ${
                  attachedCategoryFilter === 'all'
                    ? 'bg-primary text-primary-foreground shadow-xs'
                    : 'bg-accent/40 text-muted-foreground hover:text-foreground'
                }`}
              >
                All ({attachedSkills.length})
              </button>
              {(Object.keys(CATEGORY_CONFIG) as SkillCategory[]).map((cat) => {
                const count = attachedCategoryCounts[cat] || 0;
                if (count === 0) return null;
                const config = CATEGORY_CONFIG[cat];
                return (
                  <button
                    key={cat}
                    type="button"
                    onClick={() => setAttachedCategoryFilter(cat)}
                    className={`px-3 py-1 rounded-lg text-xs font-semibold transition-all cursor-pointer whitespace-nowrap ${
                      attachedCategoryFilter === cat
                        ? 'bg-primary text-primary-foreground shadow-xs'
                        : 'bg-accent/40 text-muted-foreground hover:text-foreground'
                    }`}
                  >
                    {config.label} ({count})
                  </button>
                );
              })}
            </div>

            {attachedSkills.length > 5 && (
              <div className="relative min-w-[200px]">
                <input
                  type="text"
                  value={attachedSearchQuery}
                  onChange={(e) => setAttachedSearchQuery(e.target.value)}
                  placeholder="Filter attached..."
                  className="w-full rounded-lg border border-border bg-background px-3 py-1.5 text-xs placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            )}
          </div>
        )}

        {/* Skills Cards Grid */}
        {displayedAttachedSkills.length > 0 ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
            {displayedAttachedSkills.map((s) => {
              const masterSkill = masterSkills.find((ms) => ms.id === s.masterSkillId);
              const skillName = masterSkill ? masterSkill.name : s.masterSkillId;
              const category = masterSkill?.category || 'Other';
              const catConfig = CATEGORY_CONFIG[category as SkillCategory];
              const categoryLabel = catConfig?.label || category;
              const colorClass = catConfig?.colorClass || 'bg-accent text-foreground border-border';
              const CatIcon = catConfig?.icon || Award;

              return (
                <div
                  key={s.masterSkillId}
                  className="group flex flex-col justify-between rounded-xl bg-background border border-border/80 p-4 shadow-xs hover:border-border transition-all space-y-3"
                >
                  <div className="flex items-start justify-between gap-2">
                    <div>
                      <div className="font-semibold text-sm text-foreground flex items-center gap-1.5">
                        <span>{skillName}</span>
                      </div>
                      <div
                        className={`inline-flex items-center gap-1 rounded-md px-2 py-0.5 text-[10px] font-semibold border mt-1 ${colorClass}`}
                      >
                        <CatIcon className="h-3 w-3 shrink-0" />
                        <span>{categoryLabel}</span>
                      </div>
                    </div>

                    {!isReadOnly && (
                      <button
                        type="button"
                        onClick={() => handleRemoveSkill(s.masterSkillId)}
                        className="text-muted-foreground/60 hover:text-destructive transition-colors p-1.5 rounded-lg hover:bg-destructive/10"
                        title="Remove skill from revision"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    )}
                  </div>

                  {/* Proficiency Rating Selector / Display */}
                  <div className="pt-2 border-t border-border/40 flex items-center justify-between gap-2">
                    <span className="text-[11px] font-medium text-muted-foreground">
                      Proficiency:
                    </span>
                    <div className="flex items-center gap-1">
                      {[1, 2, 3, 4, 5].map((lvl) => {
                        const isFilled = lvl <= s.proficiencyLevel;
                        return (
                          <button
                            key={lvl}
                            type="button"
                            disabled={isReadOnly}
                            onClick={() => handleUpdateProficiency(s.masterSkillId, lvl)}
                            title={`Set level to ${lvl} (${PROFICIENCY_LABELS[lvl].short})`}
                            className={`h-5 w-5 rounded flex items-center justify-center text-[10px] font-bold transition-all ${
                              isReadOnly ? 'cursor-default' : 'cursor-pointer hover:scale-110'
                            } ${
                              isFilled
                                ? 'bg-primary text-primary-foreground shadow-xs'
                                : 'bg-accent/40 text-muted-foreground/40 border border-border/40'
                            }`}
                          >
                            {lvl}
                          </button>
                        );
                      })}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        ) : (
          <div className="rounded-xl border border-dashed border-border/80 p-8 text-center space-y-3 bg-accent/5">
            <div className="mx-auto w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center">
              <Award className="h-5 w-5" />
            </div>
            <div>
              <p className="text-sm font-semibold text-foreground">No skills attached</p>
              <p className="text-xs text-muted-foreground max-w-sm mx-auto mt-1">
                Select skills from the MasterSkill catalog above to attach them to this resume revision.
              </p>
            </div>
          </div>
        )}
      </div>

      {/* SKILL FORM MODAL */}
      <SkillFormModal
        isOpen={isSkillModalOpen}
        onClose={() => setIsSkillModalOpen(false)}
        initialCategory={
          selectedCategoryFilter !== 'all'
            ? (selectedCategoryFilter as SkillCategory)
            : 'ProgrammingLanguage'
        }
      />
    </div>
  );
};

export default SkillsSection;
