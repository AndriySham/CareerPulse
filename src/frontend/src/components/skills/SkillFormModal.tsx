import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import CustomSelect from '@/components/ui/CustomSelect';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateMasterSkill } from '@/api/masterSkills';
import type { SkillCategory } from '@/types';
import { CATEGORY_CONFIG } from './SkillCard';

interface SkillFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  initialCategory?: SkillCategory;
}

const CATEGORY_OPTIONS = (Object.keys(CATEGORY_CONFIG) as SkillCategory[]).map((cat) => ({
  value: cat,
  label: CATEGORY_CONFIG[cat].label,
}));

export const SkillFormModal: React.FC<SkillFormModalProps> = ({
  isOpen,
  onClose,
  initialCategory = 'ProgrammingLanguage',
}) => {
  const createMutation = useCreateMasterSkill();

  const [name, setName] = useState('');
  const [category, setCategory] = useState<SkillCategory>(initialCategory);
  const [aliasesText, setAliasesText] = useState('');

  useEffect(() => {
    if (isOpen) {
      setName('');
      setCategory(initialCategory);
      setAliasesText('');
      createMutation.reset();
    }
  }, [isOpen, initialCategory]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim()) return;

    const aliases = aliasesText
      .split(',')
      .map((a) => a.trim())
      .filter((a) => a.length > 0);

    try {
      await createMutation.mutateAsync({
        name: name.trim(),
        category,
        aliases: aliases.length > 0 ? aliases : undefined,
      });
      onClose();
    } catch {
      // Error is caught and displayed by ErrorAlert via createMutation.error
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Add Master Skill"
      description="Create a canonical master skill entry in the normalized skill catalog (ADR 006)."
    >
      <form onSubmit={handleSubmit} autoComplete="off" className="space-y-4">
        {createMutation.error && <ErrorAlert error={createMutation.error} />}

        <div>
          <label
            htmlFor="skill-name"
            className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
          >
            Canonical Skill Name <span className="text-destructive">*</span>
          </label>
          <input
            id="skill-name"
            type="text"
            required
            maxLength={200}
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. PostgreSQL, React, Docker, EF Core"
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div>
          <label
            htmlFor="skill-category"
            className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
          >
            Skill Category <span className="text-destructive">*</span>
          </label>
          <CustomSelect
            value={category}
            onChange={(val) => setCategory(val as SkillCategory)}
            options={CATEGORY_OPTIONS}
            className="w-full"
          />
        </div>

        <div>
          <label
            htmlFor="skill-aliases"
            className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
          >
            Initial Aliases (Optional)
          </label>
          <input
            id="skill-aliases"
            type="text"
            value={aliasesText}
            onChange={(e) => setAliasesText(e.target.value)}
            placeholder="Comma-separated aliases, e.g. Postgres, PGSQL"
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
          <p className="mt-1 text-[11px] text-muted-foreground">
            Aliases map textual variations (e.g. "Postgres") to this canonical MasterSkill.
          </p>
        </div>

        <div className="flex items-center justify-end gap-3 pt-4 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            disabled={createMutation.isPending}
            className="rounded-lg px-4 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={createMutation.isPending || !name.trim()}
            className="inline-flex items-center justify-center rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-colors disabled:opacity-50 cursor-pointer"
          >
            {createMutation.isPending ? 'Creating...' : 'Create Master Skill'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default SkillFormModal;
