import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  Kanban,
  Building2,
  Briefcase,
  FileUser,
  Award,
  Activity,
  X,
} from 'lucide-react';

interface SidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
}

const navItems = [
  {
    name: 'Applications / Kanban',
    path: '/applications',
    icon: Kanban,
  },
  {
    name: 'Companies',
    path: '/companies',
    icon: Building2,
  },
  {
    name: 'Vacancies',
    path: '/vacancies',
    icon: Briefcase,
  },
  {
    name: 'Resumes',
    path: '/resumes',
    icon: FileUser,
  },
  {
    name: 'Master Skills',
    path: '/skills',
    icon: Award,
  },
];

export const Sidebar: React.FC<SidebarProps> = ({ isOpen = false, onClose }) => {
  return (
    <>
      {/* Mobile backdrop */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-background/80 backdrop-blur-sm md:hidden"
          onClick={onClose}
          aria-hidden="true"
        />
      )}

      {/* Sidebar container */}
      <aside
        className={`fixed inset-y-0 left-0 z-50 flex w-64 flex-col border-r border-border/40 bg-card/95 transition-transform duration-300 ease-in-out md:static md:translate-x-0 ${
          isOpen ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        {/* Brand logo header */}
        <div className="flex h-16 items-center justify-between px-6 border-b border-border/40">
          <div className="flex items-center gap-2.5 font-bold text-lg text-foreground tracking-tight">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary text-primary-foreground shadow-sm">
              <Activity className="h-5 w-5" />
            </div>
            <span>CareerPulse</span>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="rounded-md p-1.5 text-muted-foreground hover:bg-accent hover:text-accent-foreground md:hidden"
            aria-label="Close sidebar"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Navigation list */}
        <div className="flex-1 overflow-y-auto px-3 py-4">
          <div className="px-3 pb-2 text-[11px] font-semibold uppercase tracking-wider text-muted-foreground/70">
            Navigation
          </div>
          <nav className="space-y-1">
            {navItems.map((item) => {
              const Icon = item.icon;
              return (
                <NavLink
                  key={item.path}
                  to={item.path}
                  onClick={onClose}
                  className={({ isActive }) =>
                    `flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-all duration-200 ${
                      isActive
                        ? 'bg-primary text-primary-foreground shadow-sm font-semibold'
                        : 'text-muted-foreground hover:bg-accent/60 hover:text-foreground'
                    }`
                  }
                >
                  <Icon className="h-4 w-4 shrink-0" />
                  <span>{item.name}</span>
                </NavLink>
              );
            })}
          </nav>
        </div>

        {/* Footer info */}
        <div className="p-4 border-t border-border/40 text-xs text-muted-foreground/60 text-center">
          Career CRM v1.0 &bull; Personal SSOT
        </div>
      </aside>
    </>
  );
};

export default Sidebar;
