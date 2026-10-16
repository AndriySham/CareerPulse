import React, { useState, useMemo } from 'react';
import type { InterviewDto } from '@/types';
import { useDeleteInterview } from '@/api/interviews';
import InterviewCard from './InterviewCard';
import InterviewFormModal from './InterviewFormModal';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { Calendar, Plus, CheckCircle2, Trash2, Loader2, Sparkles } from 'lucide-react';

interface InterviewSectionProps {
  applicationId: string;
  interviews: InterviewDto[];
  isReadOnly?: boolean;
}

export const InterviewSection: React.FC<InterviewSectionProps> = ({
  applicationId,
  interviews,
  isReadOnly = false,
}) => {
  const deleteMutation = useDeleteInterview();

  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [editingInterview, setEditingInterview] = useState<InterviewDto | null>(null);

  const [interviewToDelete, setInterviewToDelete] = useState<InterviewDto | null>(null);
  const [deleteError, setDeleteError] = useState<unknown | null>(null);

  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const showSuccess = (msg: string) => {
    setSuccessMessage(msg);
    setTimeout(() => {
      setSuccessMessage(null);
    }, 4000);
  };

  // Sort interviews chronologically (by conductedAt or scheduledAt or latest)
  const sortedInterviews = useMemo(() => {
    return [...interviews].sort((a, b) => {
      const dateA = a.scheduledAt || a.conductedAt || '';
      const dateB = b.scheduledAt || b.conductedAt || '';
      return new Date(dateA).getTime() - new Date(dateB).getTime();
    });
  }, [interviews]);

  const handleOpenAdd = () => {
    setEditingInterview(null);
    setIsFormModalOpen(true);
  };

  const handleOpenEdit = (interview: InterviewDto) => {
    setEditingInterview(interview);
    setIsFormModalOpen(true);
  };

  const handleCloseFormModal = () => {
    setIsFormModalOpen(false);
    setEditingInterview(null);
  };

  const handleFormSuccess = (action: 'created' | 'updated') => {
    showSuccess(
      `Interview round ${action === 'created' ? 'added' : 'updated'} successfully.`
    );
  };

  const handleDeletePrompt = (interview: InterviewDto) => {
    setDeleteError(null);
    setInterviewToDelete(interview);
  };

  const handleCancelDelete = () => {
    if (deleteMutation.isPending) return;
    setInterviewToDelete(null);
    setDeleteError(null);
  };

  const handleConfirmDelete = async () => {
    if (!interviewToDelete) return;

    setDeleteError(null);
    try {
      await deleteMutation.mutateAsync({
        applicationId,
        interviewId: interviewToDelete.id,
      });
      setInterviewToDelete(null);
      showSuccess('Interview round removed successfully.');
    } catch (err) {
      setDeleteError(err);
    }
  };

  return (
    <div className="space-y-4">
      {/* Header toolbar */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 border-b border-border/40 pb-3">
        <div className="flex items-center gap-2.5">
          <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <Calendar className="h-4 w-4" />
          </div>
          <div>
            <h3 className="font-bold text-sm text-foreground flex items-center gap-2">
              <span>Interview Rounds</span>
              <span className="rounded-full bg-accent px-2 py-0.5 text-[11px] font-semibold text-muted-foreground">
                {interviews.length}
              </span>
            </h3>
            <p className="text-xs text-muted-foreground">
              Track screening, technical rounds, system design, and feedback.
            </p>
          </div>
        </div>

        {!isReadOnly && (
          <button
            type="button"
            onClick={handleOpenAdd}
            className="inline-flex items-center gap-1.5 rounded-lg bg-primary px-3 py-1.5 text-xs font-semibold text-primary-foreground hover:bg-primary/90 transition-all cursor-pointer shrink-0 self-start sm:self-auto"
          >
            <Plus className="h-3.5 w-3.5" />
            <span>Add Interview</span>
          </button>
        )}
      </div>

      {/* Success Notification */}
      {successMessage && (
        <div className="flex items-center gap-2 rounded-lg border border-emerald-500/30 bg-emerald-500/10 p-3 text-xs text-emerald-400 animate-in fade-in">
          <CheckCircle2 className="h-4 w-4 shrink-0" />
          <span>{successMessage}</span>
        </div>
      )}

      {/* Empty State */}
      {interviews.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-background/50 p-8 text-center space-y-3">
          <div className="mx-auto w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center">
            <Sparkles className="h-5 w-5" />
          </div>
          <div>
            <p className="text-sm font-semibold text-foreground">No interview rounds scheduled</p>
            <p className="text-xs text-muted-foreground max-w-sm mx-auto mt-1">
              Add your phone screens, technical rounds, system design challenges, or final interviews with recruiter notes.
            </p>
          </div>
          {!isReadOnly && (
            <button
              type="button"
              onClick={handleOpenAdd}
              className="inline-flex items-center gap-1.5 rounded-lg border border-primary/30 bg-primary/10 px-3.5 py-1.5 text-xs font-semibold text-primary hover:bg-primary/20 transition-all cursor-pointer mt-1"
            >
              <Plus className="h-3.5 w-3.5" />
              <span>Add First Interview</span>
            </button>
          )}
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-3">
          {sortedInterviews.map((interview) => (
            <InterviewCard
              key={interview.id}
              interview={interview}
              onEdit={handleOpenEdit}
              onDelete={handleDeletePrompt}
              isReadOnly={isReadOnly}
            />
          ))}
        </div>
      )}

      {/* Add / Edit Form Modal */}
      <InterviewFormModal
        isOpen={isFormModalOpen}
        onClose={handleCloseFormModal}
        applicationId={applicationId}
        interviewToEdit={editingInterview}
        onSuccess={handleFormSuccess}
      />

      {/* Delete Confirmation Modal */}
      <Modal
        isOpen={Boolean(interviewToDelete)}
        onClose={handleCancelDelete}
        title="Delete Interview Round"
        description="Are you sure you want to delete this interview record? This action cannot be undone."
        maxWidth="md"
      >
        <div className="space-y-4">
          {deleteError && (
            <ErrorAlert error={deleteError} onDismiss={() => setDeleteError(null)} />
          )}

          {interviewToDelete && (
            <div className="rounded-lg border border-border bg-accent/30 p-3 text-xs space-y-1">
              <div className="font-semibold text-foreground">
                Round: <span className="text-primary">{interviewToDelete.type}</span>
              </div>
              {interviewToDelete.scheduledAt && (
                <div className="text-muted-foreground">
                  Scheduled:{' '}
                  {new Date(interviewToDelete.scheduledAt).toLocaleString()}
                </div>
              )}
            </div>
          )}

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
                <>
                  <Loader2 className="h-3.5 w-3.5 animate-spin" />
                  <span>Deleting...</span>
                </>
              ) : (
                <>
                  <Trash2 className="h-3.5 w-3.5" />
                  <span>Delete Interview</span>
                </>
              )}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default InterviewSection;
