import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateVacancy, useUpdateVacancy } from '@/api/vacancies';
import { useCompanies } from '@/api/companies';
import type { VacancyDto } from '@/types';

interface VacancyFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  vacancyToEdit?: VacancyDto | null;
  initialCompanyId?: string;
}

export const VacancyFormModal: React.FC<VacancyFormModalProps> = ({
  isOpen,
  onClose,
  vacancyToEdit,
  initialCompanyId,
}) => {
  const isEditing = Boolean(vacancyToEdit);
  const createMutation = useCreateVacancy();
  const updateMutation = useUpdateVacancy();
  const { data: companies = [], isLoading: companiesLoading } = useCompanies(false);

  const [companyId, setCompanyId] = useState('');
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [url, setUrl] = useState('');
  const [postedAt, setPostedAt] = useState('');

  useEffect(() => {
    if (vacancyToEdit) {
      setCompanyId(vacancyToEdit.companyId);
      setTitle(vacancyToEdit.title || '');
      setDescription(vacancyToEdit.description || '');
      setUrl(vacancyToEdit.url || '');
      setPostedAt(
        vacancyToEdit.postedAt
          ? new Date(vacancyToEdit.postedAt).toISOString().substring(0, 10)
          : ''
      );
    } else {
      setCompanyId(initialCompanyId || (companies.length > 0 ? companies[0].id : ''));
      setTitle('');
      setDescription('');
      setUrl('');
      setPostedAt(new Date().toISOString().substring(0, 10));
    }
    createMutation.reset();
    updateMutation.reset();
  }, [vacancyToEdit, initialCompanyId, isOpen]);

  // Update default companyId when companies load if creating
  useEffect(() => {
    if (!isEditing && !companyId && companies.length > 0) {
      setCompanyId(initialCompanyId || companies[0].id);
    }
  }, [companies, companyId, isEditing, initialCompanyId]);

  const activeMutation = isEditing ? updateMutation : createMutation;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (isEditing && vacancyToEdit) {
        await updateMutation.mutateAsync({
          id: vacancyToEdit.id,
          dto: {
            title: title.trim(),
            description: description.trim() || null,
            url: url.trim() || null,
          },
        });
      } else {
        await createMutation.mutateAsync({
          companyId,
          title: title.trim(),
          description: description.trim() || null,
          url: url.trim() || null,
          postedAt: postedAt ? new Date(postedAt).toISOString() : null,
        });
      }
      onClose();
    } catch {
      // Handled by activeMutation.error
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Vacancy' : 'Create Vacancy'}
      description={
        isEditing
          ? 'Update job opportunity details and description.'
          : 'Add a new target vacancy associated with an employer.'
      }
      maxWidth="xl"
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {activeMutation.error && <ErrorAlert error={activeMutation.error} />}

        {!isEditing && (
          <div>
            <label htmlFor="vacancy-company" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
              Company <span className="text-destructive">*</span>
            </label>
            {companiesLoading ? (
              <div className="h-10 w-full rounded-lg bg-accent/40 animate-pulse" />
            ) : companies.length === 0 ? (
              <p className="text-xs text-destructive">
                No active companies found. Please create a company first.
              </p>
            ) : (
              <select
                id="vacancy-company"
                required
                value={companyId}
                onChange={(e) => setCompanyId(e.target.value)}
                disabled={Boolean(initialCompanyId)}
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary disabled:opacity-70"
              >
                {companies.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name} {c.industry ? `(${c.industry})` : ''}
                  </option>
                ))}
              </select>
            )}
          </div>
        )}

        <div>
          <label htmlFor="vacancy-title" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
            Vacancy Title <span className="text-destructive">*</span>
          </label>
          <input
            id="vacancy-title"
            type="text"
            required
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="e.g. Senior C# .NET Developer, Full-Stack Engineer"
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label htmlFor="vacancy-url" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
              Posting / Listing URL
            </label>
            <input
              id="vacancy-url"
              type="url"
              value={url}
              onChange={(e) => setUrl(e.target.value)}
              placeholder="https://linkedin.com/jobs/view/..."
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>

          {!isEditing && (
            <div>
              <label htmlFor="vacancy-posted-at" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
                Posted Date
              </label>
              <input
                id="vacancy-posted-at"
                type="date"
                value={postedAt}
                onChange={(e) => setPostedAt(e.target.value)}
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          )}
        </div>

        <div>
          <label htmlFor="vacancy-description" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
            Job Description & Requirements
          </label>
          <textarea
            id="vacancy-description"
            rows={5}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Paste vacancy text, tech stack requirements, salary range..."
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary resize-y"
          />
        </div>

        <div className="flex items-center justify-end gap-3 pt-4 border-t border-border/40">
          <button
            type="button"
            onClick={onClose}
            disabled={activeMutation.isPending}
            className="rounded-lg px-4 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={activeMutation.isPending || !title.trim() || (!isEditing && !companyId)}
            className="inline-flex items-center justify-center rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-colors disabled:opacity-50"
          >
            {activeMutation.isPending
              ? isEditing
                ? 'Saving...'
                : 'Creating...'
              : isEditing
              ? 'Save Changes'
              : 'Create Vacancy'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default VacancyFormModal;
