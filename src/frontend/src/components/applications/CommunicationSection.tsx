import React, { useState, useMemo } from 'react';
import type { ApplicationCommunicationDto } from '@/types';
import { useDeleteCommunication } from '@/api/communications';
import CommunicationCard from './CommunicationCard';
import CommunicationFormModal from './CommunicationFormModal';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { MessageSquare, Plus, CheckCircle2, Trash2, Loader2, Sparkles } from 'lucide-react';

interface CommunicationSectionProps {
  applicationId: string;
  communications: ApplicationCommunicationDto[];
  isReadOnly?: boolean;
}

export const CommunicationSection: React.FC<CommunicationSectionProps> = ({
  applicationId,
  communications,
  isReadOnly = false,
}) => {
  const deleteMutation = useDeleteCommunication();

  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [editingCommunication, setEditingCommunication] =
    useState<ApplicationCommunicationDto | null>(null);

  const [communicationToDelete, setCommunicationToDelete] =
    useState<ApplicationCommunicationDto | null>(null);
  const [deleteError, setDeleteError] = useState<unknown | null>(null);

  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const showSuccess = (msg: string) => {
    setSuccessMessage(msg);
    setTimeout(() => {
      setSuccessMessage(null);
    }, 4000);
  };

  // Sort communications newest first
  const sortedCommunications = useMemo(() => {
    return [...communications].sort((a, b) => {
      return new Date(b.occurredAt).getTime() - new Date(a.occurredAt).getTime();
    });
  }, [communications]);

  const handleOpenAdd = () => {
    setEditingCommunication(null);
    setIsFormModalOpen(true);
  };

  const handleOpenEdit = (comm: ApplicationCommunicationDto) => {
    setEditingCommunication(comm);
    setIsFormModalOpen(true);
  };

  const handleCloseFormModal = () => {
    setIsFormModalOpen(false);
    setEditingCommunication(null);
  };

  const handleFormSuccess = (action: 'created' | 'updated') => {
    showSuccess(
      `Communication record ${action === 'created' ? 'logged' : 'updated'} successfully.`
    );
  };

  const handleDeletePrompt = (comm: ApplicationCommunicationDto) => {
    setDeleteError(null);
    setCommunicationToDelete(comm);
  };

  const handleCancelDelete = () => {
    if (deleteMutation.isPending) return;
    setCommunicationToDelete(null);
    setDeleteError(null);
  };

  const handleConfirmDelete = async () => {
    if (!communicationToDelete) return;

    setDeleteError(null);
    try {
      await deleteMutation.mutateAsync({
        applicationId,
        communicationId: communicationToDelete.id,
      });
      setCommunicationToDelete(null);
      showSuccess('Communication record removed successfully.');
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
            <MessageSquare className="h-4 w-4" />
          </div>
          <div>
            <h3 className="font-bold text-sm text-foreground flex items-center gap-2">
              <span>Communication History</span>
              <span className="rounded-full bg-accent px-2 py-0.5 text-[11px] font-semibold text-muted-foreground">
                {communications.length}
              </span>
            </h3>
            <p className="text-xs text-muted-foreground">
              Log emails, phone calls, recruiter updates, and application follow-ups.
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
            <span>Log Communication</span>
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
      {communications.length === 0 ? (
        <div className="rounded-xl border border-dashed border-border/80 bg-background/50 p-8 text-center space-y-3">
          <div className="mx-auto w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center">
            <Sparkles className="h-5 w-5" />
          </div>
          <div>
            <p className="text-sm font-semibold text-foreground">No communication logged</p>
            <p className="text-xs text-muted-foreground max-w-sm mx-auto mt-1">
              Keep a timeline of responses, LinkedIn messages, recruiter interactions, and follow-ups.
            </p>
          </div>
          {!isReadOnly && (
            <button
              type="button"
              onClick={handleOpenAdd}
              className="inline-flex items-center gap-1.5 rounded-lg border border-primary/30 bg-primary/10 px-3.5 py-1.5 text-xs font-semibold text-primary hover:bg-primary/20 transition-all cursor-pointer mt-1"
            >
              <Plus className="h-3.5 w-3.5" />
              <span>Log First Communication</span>
            </button>
          )}
        </div>
      ) : (
        <div className="grid grid-cols-1 gap-3">
          {sortedCommunications.map((comm) => (
            <CommunicationCard
              key={comm.id}
              communication={comm}
              onEdit={handleOpenEdit}
              onDelete={handleDeletePrompt}
              isReadOnly={isReadOnly}
            />
          ))}
        </div>
      )}

      {/* Add / Edit Form Modal */}
      <CommunicationFormModal
        isOpen={isFormModalOpen}
        onClose={handleCloseFormModal}
        applicationId={applicationId}
        communicationToEdit={editingCommunication}
        onSuccess={handleFormSuccess}
      />

      {/* Delete Confirmation Modal */}
      <Modal
        isOpen={Boolean(communicationToDelete)}
        onClose={handleCancelDelete}
        title="Delete Communication Record"
        description="Are you sure you want to remove this communication entry from the application history?"
        maxWidth="md"
      >
        <div className="space-y-4">
          {deleteError && (
            <ErrorAlert error={deleteError} onDismiss={() => setDeleteError(null)} />
          )}

          {communicationToDelete && (
            <div className="rounded-lg border border-border bg-accent/30 p-3 text-xs space-y-1">
              <div className="font-semibold text-foreground">
                Date: {new Date(communicationToDelete.occurredAt).toLocaleString()}
              </div>
              <p className="text-muted-foreground line-clamp-2 italic">
                &quot;{communicationToDelete.notes}&quot;
              </p>
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
                  <span>Delete Communication</span>
                </>
              )}
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default CommunicationSection;
