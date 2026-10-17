import React, { useState, useMemo } from 'react';
import type { RecruiterDto } from '@/types';
import { useDeleteRecruiter } from '@/api/recruiters';
import RecruiterCard from './RecruiterCard';
import RecruiterFormModal from './RecruiterFormModal';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import {
  UserCheck,
  Plus,
  Search,
  CheckCircle2,
  Trash2,
  Loader2,
  AlertTriangle,
} from 'lucide-react';

interface RecruiterSectionProps {
  companyId: string;
  companyName: string;
  recruiters: RecruiterDto[];
  isLoading?: boolean;
  isReadOnly?: boolean;
}

export const RecruiterSection: React.FC<RecruiterSectionProps> = ({
  companyId,
  companyName,
  recruiters,
  isLoading = false,
  isReadOnly = false,
}) => {
  const deleteMutation = useDeleteRecruiter();

  const [searchQuery, setSearchQuery] = useState('');
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [editingRecruiter, setEditingRecruiter] = useState<RecruiterDto | null>(null);

  const [recruiterToDelete, setRecruiterToDelete] = useState<RecruiterDto | null>(null);
  const [deleteError, setDeleteError] = useState<unknown | null>(null);

  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const showSuccess = (msg: string) => {
    setSuccessMessage(msg);
    setTimeout(() => {
      setSuccessMessage(null);
    }, 4000);
  };

  const filteredRecruiters = useMemo(() => {
    if (!searchQuery.trim()) return recruiters;
    const query = searchQuery.toLowerCase();
    return recruiters.filter(
      (r) =>
        r.name.toLowerCase().includes(query) ||
        (r.email && r.email.toLowerCase().includes(query)) ||
        (r.phone && r.phone.toLowerCase().includes(query)) ||
        (r.notes && r.notes.toLowerCase().includes(query))
    );
  }, [recruiters, searchQuery]);

  const handleOpenAdd = () => {
    setEditingRecruiter(null);
    setIsFormModalOpen(true);
  };

  const handleOpenEdit = (recruiter: RecruiterDto) => {
    setEditingRecruiter(recruiter);
    setIsFormModalOpen(true);
  };

  const handleCloseFormModal = () => {
    setIsFormModalOpen(false);
    setEditingRecruiter(null);
  };

  const handleFormSuccess = (action: 'created' | 'updated') => {
    showSuccess(
      `Recruiter ${action === 'created' ? 'created' : 'updated'} successfully.`
    );
  };

  const handleDeletePrompt = (recruiter: RecruiterDto) => {
    setDeleteError(null);
    setRecruiterToDelete(recruiter);
  };

  const handleCancelDelete = () => {
    if (deleteMutation.isPending) return;
    setRecruiterToDelete(null);
    setDeleteError(null);
  };

  const handleConfirmDelete = async () => {
    if (!recruiterToDelete) return;

    setDeleteError(null);
    try {
      await deleteMutation.mutateAsync({
        companyId,
        recruiterId: recruiterToDelete.id,
      });
      setRecruiterToDelete(null);
      showSuccess('Recruiter removed successfully.');
    } catch (err) {
      setDeleteError(err);
    }
  };

  return (
    <div className="space-y-4">
      {/* Header bar: Title, Badge, Add button */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 border-b border-border/40 pb-3">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <UserCheck className="h-4 w-4" />
          </div>
          <div>
            <h3 className="font-bold text-sm text-foreground flex items-center gap-2">
              Recruiters
              <span className="rounded-full bg-accent px-2 py-0.5 text-xs font-semibold text-muted-foreground">
                {recruiters.length}
              </span>
            </h3>
            <p className="text-[11px] text-muted-foreground">
              Direct HR & talent contacts for {companyName}
            </p>
          </div>
        </div>

        {!isReadOnly && (
          <button
            type="button"
            onClick={handleOpenAdd}
            className="inline-flex items-center gap-1.5 rounded-lg bg-primary px-3 py-1.5 text-xs font-semibold text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer shadow-sm self-start sm:self-auto"
          >
            <Plus className="h-3.5 w-3.5" />
            <span>Add Recruiter</span>
          </button>
        )}
      </div>

      {/* Success Notification */}
      {successMessage && (
        <div className="flex items-center gap-2 rounded-lg border border-emerald-500/20 bg-emerald-500/10 px-3 py-2 text-xs font-medium text-emerald-400 animate-in fade-in duration-200">
          <CheckCircle2 className="h-3.5 w-3.5 shrink-0" />
          <span>{successMessage}</span>
        </div>
      )}

      {/* Search Filter (when more than 2 recruiters) */}
      {recruiters.length > 2 && (
        <div className="relative">
          <Search className="absolute left-3 top-2.5 h-3.5 w-3.5 text-muted-foreground" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search recruiters by name, email, phone, or notes..."
            className="w-full rounded-lg border border-border bg-background pl-8 pr-3 py-1.5 text-xs text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-primary"
          />
        </div>
      )}

      {/* Loading state */}
      {isLoading ? (
        <div className="space-y-2">
          <div className="h-20 rounded-xl bg-accent/40 animate-pulse" />
          <div className="h-20 rounded-xl bg-accent/40 animate-pulse" />
        </div>
      ) : filteredRecruiters.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 p-6 text-center space-y-2">
          <div className="mx-auto flex h-10 w-10 items-center justify-center rounded-xl bg-accent/60 text-muted-foreground">
            <UserCheck className="h-5 w-5" />
          </div>
          {searchQuery ? (
            <div className="text-xs text-muted-foreground">
              No recruiters match your search query &quot;{searchQuery}&quot;.
            </div>
          ) : (
            <div className="space-y-1">
              <p className="text-xs font-semibold text-foreground">No recruiters added yet</p>
              <p className="text-[11px] text-muted-foreground max-w-sm mx-auto">
                Keep track of talent acquisition partners, headhunters, and HR specialists representing {companyName}.
              </p>
              {!isReadOnly && (
                <div className="pt-2">
                  <button
                    type="button"
                    onClick={handleOpenAdd}
                    className="inline-flex items-center gap-1.5 rounded-lg border border-border bg-card px-3 py-1.5 text-xs font-semibold text-foreground hover:bg-accent transition-colors cursor-pointer"
                  >
                    <Plus className="h-3.5 w-3.5 text-primary" />
                    <span>Add First Recruiter</span>
                  </button>
                </div>
              )}
            </div>
          )}
        </div>
      ) : (
        <div className="space-y-3 max-h-[420px] overflow-y-auto pr-1">
          {filteredRecruiters.map((recruiter) => (
            <RecruiterCard
              key={recruiter.id}
              recruiter={recruiter}
              onEdit={!isReadOnly ? handleOpenEdit : undefined}
              onDelete={!isReadOnly ? handleDeletePrompt : undefined}
              isReadOnly={isReadOnly}
            />
          ))}
        </div>
      )}

      {/* Recruiter Create/Edit Modal */}
      <RecruiterFormModal
        isOpen={isFormModalOpen}
        onClose={handleCloseFormModal}
        companyId={companyId}
        recruiterToEdit={editingRecruiter}
        onSuccess={handleFormSuccess}
      />

      {/* Recruiter Delete Confirmation Modal */}
      <Modal
        isOpen={Boolean(recruiterToDelete)}
        onClose={handleCancelDelete}
        title="Delete Recruiter"
        description="Are you sure you want to remove this recruiter contact?"
        maxWidth="sm"
      >
        <div className="space-y-4">
          {deleteError && <ErrorAlert error={deleteError} />}

          <div className="flex items-start gap-3 rounded-xl border border-destructive/20 bg-destructive/5 p-3.5">
            <AlertTriangle className="h-5 w-5 text-destructive shrink-0 mt-0.5" />
            <div className="space-y-1 text-xs">
              <p className="font-semibold text-foreground">
                Remove {recruiterToDelete?.name}?
              </p>
              <p className="text-muted-foreground leading-relaxed">
                This will delete the recruiter record from {companyName}.
              </p>
              <p className="text-destructive font-medium pt-1">
                Note: Recruiters linked to active job applications cannot be deleted.
              </p>
            </div>
          </div>

          <div className="flex items-center justify-end gap-2 pt-2 border-t border-border/40">
            <button
              type="button"
              onClick={handleCancelDelete}
              disabled={deleteMutation.isPending}
              className="rounded-lg px-3 py-1.5 text-xs font-semibold text-muted-foreground hover:text-foreground transition-colors cursor-pointer"
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={handleConfirmDelete}
              disabled={deleteMutation.isPending}
              className="inline-flex items-center gap-1.5 rounded-lg bg-destructive px-3.5 py-1.5 text-xs font-semibold text-destructive-foreground hover:bg-destructive/90 transition-all cursor-pointer disabled:opacity-50"
            >
              {deleteMutation.isPending ? (
                <Loader2 className="h-3.5 w-3.5 animate-spin" />
              ) : (
                <Trash2 className="h-3.5 w-3.5" />
              )}
              <span>Delete Recruiter</span>
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default RecruiterSection;
