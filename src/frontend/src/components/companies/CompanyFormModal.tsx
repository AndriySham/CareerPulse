import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateCompany, useUpdateCompany } from '@/api/companies';
import type { CompanyDto } from '@/types';

interface CompanyFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  companyToEdit?: CompanyDto | null;
}

export const CompanyFormModal: React.FC<CompanyFormModalProps> = ({
  isOpen,
  onClose,
  companyToEdit,
}) => {
  const isEditing = Boolean(companyToEdit);
  const createMutation = useCreateCompany();
  const updateMutation = useUpdateCompany();

  const [name, setName] = useState('');
  const [website, setWebsite] = useState('');
  const [industry, setIndustry] = useState('');
  const [notes, setNotes] = useState('');

  useEffect(() => {
    if (companyToEdit) {
      setName(companyToEdit.name || '');
      setWebsite(companyToEdit.website || '');
      setIndustry(companyToEdit.industry || '');
      setNotes(companyToEdit.notes || '');
    } else {
      setName('');
      setWebsite('');
      setIndustry('');
      setNotes('');
    }
    createMutation.reset();
    updateMutation.reset();
  }, [companyToEdit, isOpen]);

  const activeMutation = isEditing ? updateMutation : createMutation;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (isEditing && companyToEdit) {
        await updateMutation.mutateAsync({
          id: companyToEdit.id,
          dto: {
            name: name.trim(),
            website: website.trim() || null,
            industry: industry.trim() || null,
            notes: notes.trim() || null,
          },
        });
      } else {
        await createMutation.mutateAsync({
          name: name.trim(),
          website: website.trim() || null,
          industry: industry.trim() || null,
        });
      }
      onClose();
    } catch {
      // Error handled by activeMutation.error
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title={isEditing ? 'Edit Company' : 'Add New Company'}
      description={
        isEditing
          ? 'Update the organization details and internal notes.'
          : 'Create a target employer record for your career applications.'
      }
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {activeMutation.error && <ErrorAlert error={activeMutation.error} />}

        <div>
          <label htmlFor="company-name" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
            Company Name <span className="text-destructive">*</span>
          </label>
          <input
            id="company-name"
            type="text"
            required
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="e.g. Google, Microsoft, Acme Corp"
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <label htmlFor="company-website" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
              Website URL
            </label>
            <input
              id="company-website"
              type="url"
              value={website}
              onChange={(e) => setWebsite(e.target.value)}
              placeholder="https://company.com"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>

          <div>
            <label htmlFor="company-industry" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
              Industry
            </label>
            <input
              id="company-industry"
              type="text"
              value={industry}
              onChange={(e) => setIndustry(e.target.value)}
              placeholder="e.g. FinTech, Healthcare, SaaS"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </div>

        {isEditing && (
          <div>
            <label htmlFor="company-notes" className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1">
              Internal Notes
            </label>
            <textarea
              id="company-notes"
              rows={3}
              value={notes}
              onChange={(e) => setNotes(e.target.value)}
              placeholder="Key notes, contact history, referral info..."
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary resize-none"
            />
          </div>
        )}

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
            disabled={activeMutation.isPending || !name.trim()}
            className="inline-flex items-center justify-center rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-colors disabled:opacity-50"
          >
            {activeMutation.isPending
              ? isEditing
                ? 'Saving...'
                : 'Creating...'
              : isEditing
              ? 'Save Changes'
              : 'Create Company'}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default CompanyFormModal;
