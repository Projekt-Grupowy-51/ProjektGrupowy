export const formatISODate = (isoString) => {
    const regex = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/;
    const match = isoString.match(regex);

    if (!match) {
        console.error('Invalid date format:', isoString);
        return 'Invalid Date';
    }

    const [ , year, month, day, hour, minute] = match;
    
    const date = new Date(`${year}-${month}-${day}T${hour}:${minute}:00Z`);
    
    return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        timeZone: 'UTC'
    });
};
