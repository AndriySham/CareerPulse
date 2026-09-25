import React, { useState, useEffect } from 'react';
import Modal from '@/components/ui/Modal';
import ErrorAlert from '@/components/ui/ErrorAlert';
import { useCreateProject, useUpdateProject, useProject } from '@/api/projects';
import type { ProjectDto } from '@/types';
import { Loader2 } from 'lucide-react';

interface ProjectFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  resumeRevisionId: string;
  projectToEdit?: ProjectDto | null;
  projectId?: string | null;
  onSuccess?: (action: 'created' | 'updated') => void;
}

export const ProjectFormModal: React.FC<ProjectFormModalProps> = ({
  isOpen,
  onClose,
  resumeRevisionId,
  projectToEdit,
  projectId,
  onSuccess,
}) => {
  const { data: fetchedProject, isLoading: isLoadingSingle } = useProject(
    projectId || undefined
  );

  const activeProject = projectToEdit ?? fetchedProject;
  const isEditing = Boolean(projectToEdit || projectId);

  const createMutation = useCreateProject();
  const updateMutation = useUpdateProject();

  const [projectName, setProjectName] = useState('');
  const [role, setRole] = useState('');
  const [description, setDescription] = useState('');
  const [techStack, setTechStack] = useState('');
  const [repositoryUrl, setRepositoryUrl] = useState('');
  const [liveDemoUrl, setLiveDemoUrl] = useState('');
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    if (activeProject) {
      setProjectName(activeProject.projectName || '');
      setRole(activeProject.role || '');
      setDescription(activeProject.description || '');
      setTechStack(activeProject.techStack || '');
      setRepositoryUrl(activeProject.repositoryUrl || '');
      setLiveDemoUrl(activeProject.liveDemoUrl || '');
    } else {
      setProjectName('');
      setRole('');
      setDescription('');
      setTechStack('');
      setRepositoryUrl('');
      setLiveDemoUrl('');
    }
    setValidationError(null);
    createMutation.reset();
    updateMutation.reset();
  }, [projectToEdit, activeProject, isOpen]);

  const activeMutation = isEditing ? updateMutation : createMutation;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    const trimmedProjectName = projectName.trim();
    if (!trimmedProjectName) {
      setValidationError('Project Name is required.');
      return;
    }

    if (trimmedProjectName.length > 300) {
      setValidationError('Project Name must not exceed 300 characters.');
      return;
    }

    const trimmedRole = role.trim();
    if (trimmedRole.length > 200) {
      setValidationError('Role must not exceed 200 characters.');
      return;
    }

    const trimmedTechStack = techStack.trim();
    if (trimmedTechStack.length > 1000) {
      setValidationError('Tech Stack must not exceed 1000 characters.');
      return;
    }

    const trimmedRepositoryUrl = repositoryUrl.trim();
    if (trimmedRepositoryUrl.length > 1000) {
      setValidationError('Repository URL must not exceed 1000 characters.');
      return;
    }

    const trimmedLiveDemoUrl = liveDemoUrl.trim();
    if (trimmedLiveDemoUrl.length > 1000) {
      setValidationError('Live Demo URL must not exceed 1000 characters.');
      return;
    }

    const trimmedDescription = description.trim();

    const targetId = activeProject?.id || projectId;

    try {
      if (isEditing && targetId) {
        await updateMutation.mutateAsync({
          id: targetId,
          dto: {
            projectName: trimmedProjectName,
            role: trimmedRole || null,
            description: trimmedDescription || null,
            techStack: trimmedTechStack || null,
            repositoryUrl: trimmedRepositoryUrl || null,
            liveDemoUrl: trimmedLiveDemoUrl || null,
          },
        });
        onSuccess?.('updated');
      } else {
        await createMutation.mutateAsync({
          resumeRevisionId,
          projectName: trimmedProjectName,
          role: trimmedRole || null,
          description: trimmedDescription || null,
          techStack: trimmedTechStack || null,
          repositoryUrl: trimmedRepositoryUrl || null,
          liveDemoUrl: trimmedLiveDemoUrl || null,
        });
        onSuccess?.('created');
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
      title={isEditing ? 'Edit Project' : 'Add Project'}
      description={
        isEditing
          ? 'Update project details and portfolio links for this resume revision snapshot.'
          : 'Add a portfolio project, repository, or commercial system to this resume revision.'
      }
      maxWidth="lg"
    >
      {isLoadingSingle && !activeProject ? (
        <div className="py-8 flex flex-col items-center justify-center gap-3 text-muted-foreground text-sm">
          <Loader2 className="h-6 w-6 animate-spin text-primary" />
          <span>Loading project details...</span>
        </div>
      ) : (
        <form onSubmit={handleSubmit} autoComplete="off" className="space-y-4">
          {validationError && (
            <div className="rounded-lg border border-destructive/30 bg-destructive/10 p-3 text-xs text-destructive">
              {validationError}
            </div>
          )}

          {activeMutation.error && <ErrorAlert error={activeMutation.error} />}

          {/* Project Name & Role */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <div className="flex items-center justify-between mb-1">
                <label
                  htmlFor="project-name"
                  className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground"
                >
                  Project Name <span className="text-destructive">*</span>
                </label>
                <span
                  className={`text-[10px] ${
                    projectName.length > 300 ? 'text-destructive font-bold' : 'text-muted-foreground'
                  }`}
                >
                  {projectName.length}/300
                </span>
              </div>
              <input
                id="project-name"
                type="text"
                required
                maxLength={300}
                value={projectName}
                onChange={(e) => setProjectName(e.target.value)}
                placeholder="e.g. CareerPulse, E-Commerce Platform"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <div className="flex items-center justify-between mb-1">
                <label
                  htmlFor="project-role"
                  className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground"
                >
                  Your Role
                </label>
                <span
                  className={`text-[10px] ${
                    role.length > 200 ? 'text-destructive font-bold' : 'text-muted-foreground'
                  }`}
                >
                  {role.length}/200
                </span>
              </div>
              <input
                id="project-role"
                type="text"
                maxLength={200}
                value={role}
                onChange={(e) => setRole(e.target.value)}
                placeholder="e.g. Lead Architect, Full-Stack Developer"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          </div>

          {/* Tech Stack */}
          <div>
            <div className="flex items-center justify-between mb-1">
              <label
                htmlFor="project-tech-stack"
                className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground"
              >
                Technologies / Tech Stack
              </label>
              <span
                className={`text-[10px] ${
                  techStack.length > 1000 ? 'text-destructive font-bold' : 'text-muted-foreground'
                }`}
              >
                {techStack.length}/1000
              </span>
            </div>
            <input
              id="project-tech-stack"
              type="text"
              maxLength={1000}
              value={techStack}
              onChange={(e) => setTechStack(e.target.value)}
              placeholder="e.g. C# .NET 9, PostgreSQL, React 19, Docker, Tailwind CSS"
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <p className="text-[11px] text-muted-foreground mt-1">
              Comma-separated list of frameworks, languages, databases, or libraries used.
            </p>
          </div>

          {/* Description */}
          <div>
            <label
              htmlFor="project-description"
              className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground mb-1"
            >
              Description & Highlights
            </label>
            <textarea
              id="project-description"
              rows={3}
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Describe the system architecture, business value, challenges solved, or key engineering decisions..."
              className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary resize-y"
            />
          </div>

          {/* URLs */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div>
              <div className="flex items-center justify-between mb-1">
                <label
                  htmlFor="project-repo-url"
                  className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground"
                >
                  Repository URL
                </label>
                <span
                  className={`text-[10px] ${
                    repositoryUrl.length > 1000 ? 'text-destructive font-bold' : 'text-muted-foreground'
                  }`}
                >
                  {repositoryUrl.length}/1000
                </span>
              </div>
              <input
                id="project-repo-url"
                type="url"
                maxLength={1000}
                value={repositoryUrl}
                onChange={(e) => setRepositoryUrl(e.target.value)}
                placeholder="https://github.com/user/project"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <div>
              <div className="flex items-center justify-between mb-1">
                <label
                  htmlFor="project-demo-url"
                  className="block text-xs font-semibold uppercase tracking-wider text-muted-foreground"
                >
                  Live Demo URL
                </label>
                <span
                  className={`text-[10px] ${
                    liveDemoUrl.length > 1000 ? 'text-destructive font-bold' : 'text-muted-foreground'
                  }`}
                >
                  {liveDemoUrl.length}/1000
                </span>
              </div>
              <input
                id="project-demo-url"
                type="url"
                maxLength={1000}
                value={liveDemoUrl}
                onChange={(e) => setLiveDemoUrl(e.target.value)}
                placeholder="https://myproject.demo.com"
                className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm placeholder:text-muted-foreground/60 focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          </div>

          {/* Actions Footer */}
          <div className="flex items-center justify-end gap-3 pt-4 border-t border-border/40">
            <button
              type="button"
              onClick={onClose}
              disabled={activeMutation.isPending}
              className="rounded-lg px-4 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground transition-colors disabled:opacity-50 cursor-pointer"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={activeMutation.isPending || !projectName.trim()}
              className="inline-flex items-center justify-center gap-1.5 rounded-lg bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground shadow-sm hover:bg-primary/90 transition-colors disabled:opacity-50 cursor-pointer"
            >
              {activeMutation.isPending && <Loader2 className="h-4 w-4 animate-spin" />}
              <span>
                {activeMutation.isPending
                  ? isEditing
                    ? 'Saving...'
                    : 'Adding...'
                  : isEditing
                  ? 'Save Changes'
                  : 'Add Project'}
              </span>
            </button>
          </div>
        </form>
      )}
    </Modal>
  );
};

export default ProjectFormModal;
