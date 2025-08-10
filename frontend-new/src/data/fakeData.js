// Fejkowe kolekcje danych dla wszystkich komponentów
// Te dane będą później zastąpione przez prawdziwe API calls

// Projekty
export const FAKE_PROJECTS = [
  {
    id: 1,
    name: 'Projekt Analiza Wideo 1',
    description: 'Projekt do analizy materiałów wideo edukacyjnych',
    createdAt: '2024-01-15T10:00:00Z',
    updatedAt: '2024-02-01T12:30:00Z',
    finished: false
  },
  {
    id: 2,
    name: 'Projekt Badania Behawioralne',
    description: 'Analiza zachowań w środowisku edukacyjnym',
    createdAt: '2024-02-10T14:30:00Z',
    updatedAt: '2024-03-05T09:15:00Z',
    finished: true
  },
  {
    id: 3,
    name: 'Projekt Testowy',
    description: 'Projekt służący do testowania funkcjonalności',
    createdAt: '2024-03-05T09:15:00Z',
    updatedAt: '2024-03-10T16:20:00Z',
    finished: false
  }
];

// Przedmioty
export const FAKE_SUBJECTS = [
  {
    id: 1,
    name: 'Matematyka - Algebra',
    description: 'Podstawy algebry dla uczniów szkół średnich',
    projectId: 1
  },
  {
    id: 2,
    name: 'Historia - Średniowiecze',
    description: 'Historia Europy w okresie średniowiecza',
    projectId: 1
  },
  {
    id: 3,
    name: 'Biologia - Genetyka',
    description: 'Podstawy genetyki i dziedziczności',
    projectId: 2
  }
];

// Etykiety
export const FAKE_LABELS = [
  {
    id: 1,
    name: 'Równania liniowe',
    description: 'Etykieta dla równań liniowych',
    color: '#3498db',
    subjectId: 1
  },
  {
    id: 2,
    name: 'Funkcje kwadratowe',
    description: 'Etykieta dla funkcji kwadratowych',
    color: '#e74c3c',
    subjectId: 1
  },
  {
    id: 3,
    name: 'Wojny krzyżowe',
    description: 'Etykieta dotycząca wojen krzyżowych',
    color: '#f39c12',
    subjectId: 2
  },
  {
    id: 4,
    name: 'DNA i RNA',
    description: 'Etykieta dotycząca kwasów nukleinowych',
    color: '#27ae60',
    subjectId: 3
  }
];

// Grupy video
export const FAKE_VIDEO_GROUPS = [
  {
    id: 1,
    name: 'Grupa Video Matematyka',
    description: 'Zbiór materiałów wideo z matematyki',
    projectId: 1
  },
  {
    id: 2,
    name: 'Grupa Video Historia',
    description: 'Materiały historyczne w formie wideo',
    projectId: 1
  },
  {
    id: 3,
    name: 'Grupa Video Biologia',
    description: 'Filmy edukacyjne z biologii',
    projectId: 2
  }
];

// Wideo
export const FAKE_VIDEOS = [
  {
    id: 1,
    name: 'Równania liniowe - wprowadzenie',
    description: 'Podstawy równań liniowych',
    url: 'https://example.com/video1.mp4',
    duration: 1200,
    videoGroupId: 1,
    title: 'Równania liniowe - wprowadzenie',
    positionInQueue: 1
  },
  {
    id: 2,
    name: 'Funkcje kwadratowe',
    description: 'Analiza funkcji kwadratowych',
    url: 'https://example.com/video2.mp4',
    duration: 1800,
    videoGroupId: 1,
    title: 'Funkcje kwadratowe',
    positionInQueue: 2
  },
  {
    id: 3,
    name: 'Średniowieczne zamki',
    description: 'Historia budowy zamków',
    url: 'https://example.com/video3.mp4',
    duration: 2400,
    videoGroupId: 2,
    title: 'Średniowieczne zamki',
    positionInQueue: 1
  }
];

// Przypisane etykiety do video
export const FAKE_VIDEO_ASSIGNED_LABELS = [
  {
    id: 1,
    videoId: 1,
    labelId: 1,
    labelName: 'Równania liniowe',
    labelColor: '#3498db',
    timestamp: 120,
    duration: 30
  },
  {
    id: 2,
    videoId: 1,
    labelId: 2,
    labelName: 'Funkcje kwadratowe',
    labelColor: '#e74c3c',
    timestamp: 180,
    duration: 45
  },
  {
    id: 3,
    videoId: 2,
    labelId: 2,
    labelName: 'Funkcje kwadratowe',
    labelColor: '#e74c3c',
    timestamp: 60,
    duration: 120
  }
];

// Użytkownicy (labelerzy)
export const FAKE_USERS = [
  {
    id: 1,
    name: 'Jan Kowalski',
    email: 'jan.kowalski@example.com',
    role: 'labeler'
  },
  {
    id: 2,
    name: 'Anna Nowak',
    email: 'anna.nowak@example.com',
    role: 'labeler'
  },
  {
    id: 3,
    name: 'Piotr Wiśniewski',
    email: 'piotr.wisniewski@example.com',
    role: 'admin'
  }
];

// Kody dostępu
export const FAKE_ACCESS_CODES = [
  {
    id: 1,
    code: 'MATH2024',
    description: 'Kod dostępu dla przedmiotu matematyki',
    projectId: 1,
    expiresAt: '2024-12-31T23:59:59Z',
    isActive: true
  },
  {
    id: 2,
    code: 'HIST2024',
    description: 'Kod dostępu dla przedmiotu historii',
    projectId: 1,
    expiresAt: '2024-12-31T23:59:59Z',
    isActive: true
  },
  {
    id: 3,
    code: 'BIO2024',
    description: 'Kod dostępu dla przedmiotu biologii',
    projectId: 2,
    expiresAt: '2024-12-31T23:59:59Z',
    isActive: false
  }
];

// Przypisania (assignments)
export const FAKE_ASSIGNMENTS = [
  {
    id: 1,
    userId: 1,
    videoGroupId: 1,
    projectId: 1,
    assignedAt: '2024-08-01T10:00:00Z',
    status: 'in_progress'
  },
  {
    id: 2,
    userId: 2,
    videoGroupId: 2,
    projectId: 1,
    assignedAt: '2024-08-05T14:30:00Z',
    status: 'completed'
  },
  {
    id: 3,
    userId: 1,
    videoGroupId: 3,
    projectId: 2,
    assignedAt: '2024-08-08T09:15:00Z',
    status: 'pending'
  }
];

// Funkcje pomocnicze do zarządzania danymi
export const findById = (collection, id) => {
  return collection.find(item => item.id === parseInt(id));
};

export const findByProperty = (collection, property, value) => {
  return collection.filter(item => item[property] === value);
};

export const addToCollection = (collection, newItem) => {
  const id = Math.max(...collection.map(item => item.id), 0) + 1;
  const itemWithId = { ...newItem, id };
  collection.push(itemWithId);
  return itemWithId;
};

export const updateInCollection = (collection, id, updateData) => {
  const index = collection.findIndex(item => item.id === parseInt(id));
  if (index !== -1) {
    collection[index] = { ...collection[index], ...updateData };
    return collection[index];
  }
  return null;
};

export const removeFromCollection = (collection, id) => {
  const index = collection.findIndex(item => item.id === parseInt(id));
  if (index !== -1) {
    return collection.splice(index, 1)[0];
  }
  return null;
};

// Raporty projektów
export const FAKE_PROJECT_REPORTS = [
  {
    id: 1,
    name: 'Progress Report January 2024',
    type: 'progress',
    projectId: 1,
    createdAt: '2024-01-30T10:00:00Z',
    url: '/api/projectreport/download/1'
  },
  {
    id: 2,
    name: 'Weekly Summary Report',
    type: 'summary',
    projectId: 1,
    createdAt: '2024-02-15T14:30:00Z',
    url: '/api/projectreport/download/2'
  },
  {
    id: 3,
    name: 'Behavioral Analysis Report',
    type: 'analysis',
    projectId: 2,
    createdAt: '2024-03-01T09:15:00Z',
    url: '/api/projectreport/download/3'
  },
  {
    id: 4,
    name: 'Final Project Report',
    type: 'final',
    projectId: 2,
    createdAt: '2024-03-10T16:45:00Z',
    url: '/api/projectreport/download/4'
  }
];
