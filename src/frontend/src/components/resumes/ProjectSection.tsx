import React, { useState } from 'react';
import type { ProjectDto } from '@/types';
import { useProjects, useDeleteProject } from '@/api/projects';
import ProjectCard from './ProjectCard';
import ProjectFormModal from './ProjectFormModal';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { FolderGit2, Plus, Sparkles, CheckCircle2, Trash2, Loader2 } from 'lucide-react';

interface ProjectSectionProps {
  resumeRevisionId?: string;
  isReadOnly?: boolean;
  isNewMode?: boolean;
}

export const ProjectSection: React.FC<ProjectSectionProps> = ({
  resumeRevisionId,
  isReadOnly = false,
  isNewMode = false,
}) => {
  const {
    data: projects = [],
    isLoading,
    isError,
    error,
  } = useProjects(isNewMode ? undefined : resumeRevisionId);

  const deleteMutation = useDeleteProject();

  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [editingProject, setEditingProject] = useState<ProjectDto | null>(null);

  const [projectToDelete, setProjectToDelete] = useState<ProjectDto | null>(null);
  const [deleteError, setDeleteError] = useState<unknown | null>(null);

  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const showSuccess = (msg: string) => {
    setSuccessMessage(msg);
    setTimeout(() => {
      setSuccessMessage(null);
    }, 4000);
  };

  const handleOpenAdd = () => {
    setEditingProject(null);
    setIsFormModalOpen(true);
  };

  const handleOpenEdit = (project: ProjectDto) => {
    setEditingProject(project);
    setIsFormModalOpen(true);
  };

  const handleCloseFormModal = () => {
    setIsFormModalOpen(false);
    setEditingProject(null);
  };

  const handleFormSuccess = (action: 'created' | 'updated') => {
    showSuccess(
      `Project ${action === 'created' ? 'created' : 'updated'} successfully.`
    );
  };

  const handleDeletePrompt = (project: ProjectDto) => {
    setDeleteError(null);
    setProjectToDelete(project);
  };

  const handleCancelDelete = () => {
    if (deleteMutation.isPending) return;
    setProjectToDelete(null);
    setDeleteError(null);
  };

  const handleConfirmDelete = async () => {
    if (!projectToDelete) return;

    setDeleteError(null);
    try {
      await deleteMutation.mutateAsync(projectToDelete.id);
      setProjectToDelete(null);
      showSuccess('Project deleted successfully.');
    } catch (err) {
      // Backend returned error (e.g. 409 Conflict when revision is used in an application)
      // We keep the Project visible in the UI and show the error message.
      setDeleteError(err);
    }
  };

  return (
    <div className="rounded-xl border border-border/60 bg-card p-6 shadow-sm space-y-6">
      {/* Section Header */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 border-b border-border/40 pb-4">
        <div className="flex items-center gap-3">
          <div className="p-2 rounded-xl bg-primary/10 text-primary border border-primary/20">
            <FolderGit2 className="h-5 w-5" />
          </div>
          <div>
            <h2 className="text-base font-semibold text-foreground">Projects & Portfolio</h2>
            <p className="text-xs text-muted-foreground">
              Pet projects, commercial works, open source contributions, and architecture samples (ADR 005 & ADR 010).
            </p>
          </div>
        </div>

        <div className="flex items-center gap-2">
          {!isNewMode && (
            <span className="rounded-full bg-primary/10 border border-primary/20 px-3 py-1 text-xs font-semibold text-primary">
              {projects.length} project(s)
            </span>
          )}

          {!isReadOnly && !isNewMode && (
            <button
              type="button"
              onClick={handleOpenAdd}
              className="inline-flex items-center gap-1.5 rounded-xl bg-primary px-3.5 py-1.5 text-xs font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-all cursor-pointer"
            >
              <Plus className="h-3.5 w-3.5" />
              <span>Add Project</span>
            </button>
          )}
        </div>
      </div>

      {/* Success Banner */}
      {successMessage && (
        <div className="flex items-center justify-between gap-3 rounded-xl border border-emerald-500/30 bg-emerald-500/10 p-4 text-emerald-400 text-sm font-medium animate-in fade-in">
          <div className="flex items-center gap-2.5">
            <CheckCircle2 className="h-5 w-5 shrink-0" />
            <span>{successMessage}</span>
          </div>
          <button
            type="button"
            onClick={() => setSuccessMessage(null)}
            className="text-xs text-emerald-400/70 hover:text-emerald-300 cursor-pointer"
          >
            Dismiss
          </button>
        </div>
      )}

      {/* When in New Mode (Resume Draft not created yet) */}
      {isNewMode ? (
        <div className="rounded-xl border border-dashed border-border/80 p-8 text-center space-y-3 bg-accent/5">
          <div className="mx-auto w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center">
            <Sparkles className="h-5 w-5" />
          </div>
          <div>
            <p className="text-sm font-semibold text-foreground">Save Profile to Add Projects</p>
            <p className="text-xs text-muted-foreground max-w-md mx-auto mt-1">
              Project entries are attached to versioned resume revision snapshots. Please complete and create
              your initial resume profile above first. Once saved, you can add portfolio projects here.
            </p>
          </div>
        </div>
      ) : (
        <>
          {/* Query Error Alert */}
          {isError && <ErrorAlert error={error} />}

          {/* Persistent Delete Error Banner if modal was dismissed */}
          {deleteError && !projectToDelete && (
            <ErrorAlert error={deleteError} onDismiss={() => setDeleteError(null)} />
          )}

          {/* Loading Skeletons */}
          {isLoading && (
            <div className="space-y-3">
              {[1, 2].map((i) => (
                <div
                  key={i}
                  className="rounded-xl border border-border/40 bg-accent/10 p-4 animate-pulse flex items-center gap-3"
                >
                  <div className="w-9 h-9 rounded-lg bg-accent/40" />
                  <div className="flex-1 space-y-2">
                    <div className="h-4 bg-accent/40 rounded w-1/3" />
                    <div className="h-3 bg-accent/30 rounded w-1/2" />
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Empty State */}
          {!isLoading && !isError && projects.length === 0 && (
            <div className="rounded-xl border border-dashed border-border/80 p-8 text-center space-y-3 bg-accent/5">
              <div className="mx-auto w-10 h-10 rounded-full bg-primary/10 text-primary flex items-center justify-center">
                <FolderGit2 className="h-5 w-5" />
              </div>
              <div>
                <p className="text-sm font-semibold text-foreground">No portfolio projects added</p>
                <p className="text-xs text-muted-foreground max-w-sm mx-auto mt-1">
                  Add personal projects, open source libraries, commercial systems, or repositories to showcase your practical engineering capabilities.
                </p>
              </div>
              {!isReadOnly && (
                <button
                  type="button"
                  onClick={handleOpenAdd}
                  className="inline-flex items-center gap-1.5 rounded-lg border border-primary/30 bg-primary/10 px-3.5 py-1.5 text-xs font-semibold text-primary hover:bg-primary/20 transition-all cursor-pointer mt-2"
                >
                  <Plus className="h-3.5 w-3.5" />
                  <span>Add First Project</span>
                </button>
              )}
            </div>
          )}

          {/* Projects List */}
          {!isLoading && !isError && projects.length > 0 && (
            <div className="grid grid-cols-1 gap-3">
              {projects.map((proj) => (
                <ProjectCard
                  key={proj.id}
                  project={proj}
                  onEdit={handleOpenEdit}
                  onDelete={handleDeletePrompt}
                  isReadOnly={isReadOnly}
                />
              ))}
            </div>
          )}
        </>
      )}

      {/* Form Modal (Create / Edit) */}
      {resumeRevisionId && (
        <ProjectFormModal
          isOpen={isFormModalOpen}
          onClose={handleCloseFormModal}
          resumeRevisionId={resumeRevisionId}
          projectToEdit={editingProject}
          onSuccess={handleFormSuccess}
        />
      )}

      {/* Delete Confirmation Modal */}
      <Modal
        isOpen={Boolean(projectToDelete)}
        onClose={handleCancelDelete}
        title="Delete Project Record"
        description="Please confirm you want to remove this project from the resume revision snapshot."
        maxWidth="md"
      >
        <div className="space-y-4">
          {deleteError && (
            <ErrorAlert error={deleteError} onDismiss={() => setDeleteError(null)} />
          )}

          {projectToDelete && (
            <div className="rounded-lg border border-border/60 bg-accent/20 p-3 space-y-1 text-sm">
              <div className="font-semibold text-foreground flex items-center gap-2">
                <FolderGit2 className="h-4 w-4 text-primary" />
                <span>{projectToDelete.projectName}</span>
                {projectToDelete.role && (
                  <span className="text-xs text-muted-foreground font-normal">
                    ({projectToDelete.role})
                  </span>
                )}
              </div>
              {projectToDelete.description && (
                <p className="text-xs text-muted-foreground">{projectToDelete.description}</p>
              )}
              {projectToDelete.techStack && (
                <p className="text-[11px] text-muted-foreground">
                  Stack: {projectToDelete.techStack}
                </p>
              )}
            </div>
          )}

          <p className="text-xs text-muted-foreground">
            This action will permanently delete this project from this draft revision.
            If this revision is already linked to an application, the deletion will be rejected by the server.
          </p>

          <div className="flex items-center justify-end gap-3 pt-3 border-t border-border/40">
            <button
              type="button"
              onClick={handleCancelDelete}
              disabled={deleteMutation.isPending}
              className="rounded-lg px-4 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50 cursor-pointer"
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={handleConfirmDelete}
              disabled={deleteMutation.isPending}
              className="inline-flex items-center justify-center gap-1.5 rounded-lg bg-destructive px-4 py-2 text-sm font-semibold text-destructive-foreground shadow-sm hover:bg-destructive/90 transition-colors disabled:opacity-50 cursor-pointer"
            >
              {deleteMutation.isPending ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                <Trash2 className="h-4 w-4" />
              )}
              <span>{deleteMutation.isPending ? 'Deleting...' : 'Delete Project'}</span>
            </button>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default ProjectSection;
